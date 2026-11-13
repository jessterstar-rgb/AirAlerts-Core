public static class AlertEndpoints
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/", () => "Hello World!");

        app.MapGet("/ping", () => "pong");

        app.MapGet("/alerts", () => AlertState.Alerts.Select(a => new AlertDto(a.Key, a.Value)));

        app.MapGet("/alerts/{region}", IResult (string region) =>
        {
            var found = AlertState.Alerts.TryGetValue(region, out var active);

            if (found)
            {
                return Results.Ok(new AlertDto(region, active));
            }
            
            return Results.NotFound();
        });

        app.MapGet("/search", (string name = "Kyiv") => $"Looking for {name}");

        app.MapPost("/alerts", async (AlertDto dto) =>
        {
            AlertState.Alerts[dto.Region] = dto.Active;
            await AlertState.Broadcast(new { type = "change", data = dto });
            return Results.Ok(dto);
        });
    }
}