namespace AlertsApi.Endpoints;

using System.Text.Json;
using AlertsApi.Models;
using AlertsApi.Services;

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

        app.MapGet("/alerts", AlertState.GetAll);

        app.MapGet("/alerts/{regionId}", IResult (string regionId) =>
        {
            var alert = AlertState.Get(regionId);
            return alert is null ? Results.NotFound() : Results.Ok(alert);
        });

        app.MapPost("/alerts", async Task<IResult> (SetAlertRequest request) =>
        {
            var alert = await AlertState.SetActiveAsync(request.RegionId, request.IsActive);
            return alert is null ? Results.NotFound() : Results.Ok(alert);
        });
    }
}
