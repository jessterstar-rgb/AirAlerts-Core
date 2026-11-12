using System.Net.WebSockets;
using System.Text.Json;
using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var alerts = new ConcurrentDictionary<string, bool>
{
    ["Kyiv"] = false,
    ["Lviv"] = true
};
var clients = new ConcurrentDictionary<Guid, WebSocket>();
var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

async Task Broadcast(object message)
{
    var bytes = JsonSerializer.SerializeToUtf8Bytes(message, jsonOptions);
    foreach (var (id, socket) in clients)
    {
        try
        {
            if (socket.State == WebSocketState.Open)
                await socket.SendAsync(bytes, WebSocketMessageType.Text, true, CancellationToken.None);
            else
                clients.TryRemove(id, out _);
        }
        catch
        {
            clients.TryRemove(id, out _);
        }
    }
}

app.UseWebSockets();

app.Map("/ws", async (HttpContext ctx) =>
{
    if (!ctx.WebSockets.IsWebSocketRequest)
    {
        ctx.Response.StatusCode = 400;
        return;
    }

    using var ws = await ctx.WebSockets.AcceptWebSocketAsync();
    var id = Guid.NewGuid();
    clients[id] = ws;

    try
    {
        var snapshot = JsonSerializer.SerializeToUtf8Bytes(
            new { type = "snapshot", data = alerts }, jsonOptions);
        await ws.SendAsync(snapshot, WebSocketMessageType.Text, true, ctx.RequestAborted);

        var buffer = new byte[1024];
        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(buffer, ctx.RequestAborted);
            if (result.MessageType == WebSocketMessageType.Close) break;
        }
    }
    catch (OperationCanceledException) { }
    catch (WebSocketException) { }
    finally
    {
        clients.TryRemove(id, out _);
    }
});

app.MapGet("/", () => "Hello World!");

app.MapGet("/ping",  () => "pong");

app.MapGet("/alerts", () => alerts.Select(a => new AlertDto(a.Key, a.Value)));

app.MapGet("/alerts/{regionId}", () =>
    new {  Region = "Kyiv", Active = true }
);

app.MapGet("/search", (string name = "Kyiv") => $"Looking for {name}");

app.MapPost("/alerts", async (AlertDto dto) =>
{
    alerts[dto.Region] = dto.Active;
    await Broadcast(new { type = "change", data = dto});
    return Results.Ok(dto);
});

app.Run();

record AlertDto(string Region, bool Active);