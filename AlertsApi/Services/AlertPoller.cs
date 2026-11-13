namespace AlertsApi.Services;

public class AlertPoller : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(20);
    private readonly AlertsInUaClient _client;

    public AlertPoller(AlertsInUaClient client)
    {
        _client = client;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await _client.GetActiveAlertsAsync(stoppingToken);

                if (response is not null)
                {
                    var byOblast = response.Alerts
                        .Where(a => a.AlertType == "air_raid")
                        .GroupBy(a => a.LocationOblast)
                        .Select(g => $"{g.Key}: {g.Count()}");

                    Console.WriteLine($"[{DateTime.Now:T}] {string.Join(" | ", byOblast)}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка опитування: {ex.Message}");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }
    }
}
