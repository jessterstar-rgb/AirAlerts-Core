namespace AlertsApi.Services;

public class AlertSimulator : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);

            var regions = AlertState.Alerts.Keys.ToList();

            if (regions.Count == 0)
            {
                continue;
            }

            var randomRegion = regions[Random.Shared.Next(regions.Count)];
            var newState = !AlertState.Alerts[randomRegion];

            AlertState.Alerts[randomRegion] = newState;

            var changeData = new { region = randomRegion, active = newState };
            await AlertState.Broadcast(new { type = "change", data = changeData });
        }
    }
}
