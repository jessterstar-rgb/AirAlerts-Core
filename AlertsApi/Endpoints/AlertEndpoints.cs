namespace AlertsApi.Endpoints;

using System.Text.Json;
using AlertsApi.Models;

public static class AlertEndpoints
{
    private static readonly JsonSerializerOptions SnakeCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };
    public static void Map(WebApplication app)
    {
        app.MapGet("/", () => "Hello World!");

        app.MapGet("/ping", () => "pong");

        app.MapGet("/alerts", () => AlertState.Alerts.Select(a => new AlertDto(a.Key, a.Value)));

        app.MapGet("/alerts/{region}", IResult (string region) =>
        {
            var found = AlertState.Alerts.TryGetValue(region, out var active);

            return found ? Results.Ok(new AlertDto(region, active)) : Results.NotFound();
        });

        app.MapGet("/search", (string region) =>
        {
            var found = AlertState.Alerts.TryGetValue(region, out var active);

            return found ? Results.Ok(new AlertDto(region, active)) : Results.NotFound();
        });

        app.MapPost("/alerts", async (AlertDto dto) =>
        {
            AlertState.Alerts[dto.Region] = dto.Active;
            await AlertState.Broadcast(new { type = "change", data = dto });

            return Results.Ok(dto);
        });

        // temporary test endpoint
        // app.MapGet("/debug/parse", () =>
        //{
          //  var text = File.ReadAllText("/Users/shlazov/personal/response.json");

            //var response = JsonSerializer.Deserialize<AlertsInUaResponse>(text, SnakeCaseOptions);

            //var byOblast = response!.Alerts
              //  .Where(a => a.AlertType == "air_raid")
               // .GroupBy(a => a.LocationOblast)
               // .Select(g => new { Oblast = g.Key, Count = g.Count() });

            //return new
            //{
              //  Total = response.Alerts.Count,
              //  AirRaidByOblast = byOblast
            //};
        //});
    }
}
