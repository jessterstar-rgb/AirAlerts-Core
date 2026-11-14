using System.Net.Http.Json;
using System.Text.Json;
using AlertsApi.Models;

namespace AlertsApi.Services;

public class AlertsInUaClient(HttpClient httpClient)
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    public Task<AlertsInUaResponse?> GetActiveAlertsAsync(CancellationToken ct) =>
        httpClient.GetFromJsonAsync<AlertsInUaResponse>("v1/alerts/active.json", Options, ct);
}
