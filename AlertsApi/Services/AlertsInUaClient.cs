using System.Net.Http.Json;
using System.Text.Json;
using AlertsApi.Models;

namespace AlertsApi.Services;

public class AlertsInUaClient
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
    };

    private readonly HttpClient _httpClient;

    public AlertsInUaClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public Task<AlertsInUaResponse?> GetActiveAlertsAsync(CancellationToken ct) =>
        _httpClient.GetFromJsonAsync<AlertsInUaResponse>("v1/alerts/active.json", Options, ct);
}
