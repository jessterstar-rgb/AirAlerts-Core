namespace AlertsApi.Endpoints;

using System.Net.WebSockets;
using System.Text.Json;

public static class WebSocketEndpoints
{
    public static void Map(WebApplication app)
    {
        app.Map("/ws", async (HttpContext ctx) =>
        {
            if (!ctx.WebSockets.IsWebSocketRequest)
            {
                ctx.Response.StatusCode = 400;
                return;
            }

            using var ws = await ctx.WebSockets.AcceptWebSocketAsync();
            var id = Guid.NewGuid();
            AlertState.Clients[id] = ws;

            try
            {
                var snapshot = JsonSerializer.SerializeToUtf8Bytes(
                    new { type = "snapshot", data = AlertState.Alerts },
                    AlertState.JsonOptions
                );
                await ws.SendAsync(snapshot, WebSocketMessageType.Text, true, ctx.RequestAborted);

                var buffer = new byte[1024];
                while (ws.State == WebSocketState.Open)
                {
                    var result = await ws.ReceiveAsync(buffer, ctx.RequestAborted);
                    if (result.MessageType == WebSocketMessageType.Close)
                        break;
                }
            }
            catch (OperationCanceledException) { }
            catch (WebSocketException) { }
            finally
            {
                AlertState.Clients.TryRemove(id, out _);
            }
        });
    }
}
