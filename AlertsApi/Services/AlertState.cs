using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text.Json;

public static class AlertState
{
    public static readonly ConcurrentDictionary<string, bool> Alerts = new()
    {
        ["Kyiv"] = false,
        ["Lviv"] = true,
    };

    public static readonly ConcurrentDictionary<Guid, WebSocket> Clients = new();

    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    public static async Task Broadcast(object message)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(message, JsonOptions);
        foreach (var (id, socket) in Clients)
        {
            try
            {
                if (socket.State == WebSocketState.Open)
                    await socket.SendAsync(
                        bytes,
                        WebSocketMessageType.Text,
                        true,
                        CancellationToken.None
                    );
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
