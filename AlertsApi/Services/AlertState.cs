namespace AlertsApi.Services;

using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;
using AlertsApi.Models;

public static class AlertState
{
    public static readonly ConcurrentDictionary<string, bool> Alerts = new(
        Regions.All.Select(r => new KeyValuePair<string, bool>(r.Id, false)));

    public static readonly ConcurrentDictionary<Guid, WebSocket> Clients = new();

    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    // увесь стан як масив, у порядку довідника
    public static IReadOnlyList<AlertDto> GetAll() =>
        [.. Regions.All.Select(r => new AlertDto(r.Id, r.Name, Alerts[r.Id]))];

    public static AlertDto? Get(string regionId)
    {
        var region = Regions.FindById(regionId);
        return region is null
            ? null
            : new AlertDto(region.Id, region.Name, Alerts[region.Id]);
    }

    // null, якщо такого регіону нема; розсилає change лише коли стан справді змінився
    public static async Task<AlertDto?> SetActiveAsync(string regionId, bool isActive)
    {
        var region = Regions.FindById(regionId);

        if (region is null) return null;

        var dto = new AlertDto(region.Id, region.Name, isActive);

        if (Alerts[region.Id] == isActive) return dto;

        Alerts[region.Id] = isActive;

        await Broadcast(new { type = "change", data = dto });

        return dto;
    }

    public static async Task Broadcast(object message)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);
        foreach (var (id, socket) in Clients)
        {
            try
            {
                if (socket.State == WebSocketState.Open)
                    await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
                else
                    Clients.TryRemove(id, out _);
            }
            catch
            {
                Clients.TryRemove(id, out _);
            }
        }
    }
}
