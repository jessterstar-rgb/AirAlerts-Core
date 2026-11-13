public static class AlertEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/", () => "Hello World!");

        app.MapGet("/ping", () => "pong");

        app.MapGet("/alerts", () => AlertState.Alerts.Select(a => new AlertDto(a.Key, a.Value)));

        app.MapGet("/alerts/{regionId}", () =>
            new { Region = "Kyiv", Active = true }
        );

        app.MapGet("/search", (string name = "Kyiv") => $"Looking for {name}");

        app.MapPost("/alerts", async (AlertDto dto) =>
        {
            AlertState.Alerts[dto.Region] = dto.Active;
            await AlertState.Broadcast(new { type = "change", data = dto });
            return Results.Ok(dto);
        });
    }
}