using AlertsApi.Models;

namespace AlertsApi.Services;

public class AlertPoller(AlertsInUaClient client) : BackgroundService
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromSeconds(20);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var response = await client.GetActiveAlertsAsync(stoppingToken);

                if (response is not null)
                {

                    var activeNames = response.Alerts
                        .Where(a => a.AlertType == "air_raid")
                        .Select(a => a.LocationOblast)
                        .ToHashSet();

                    foreach (var activeRegion in activeNames.Where(activeRegion => Regions.FindByName(activeRegion) is null))
                    {
                        Console.WriteLine($"Region {activeRegion} not found in the Regions dictionary");
                    }

                    foreach (var region in Regions.All)
                    {
                        await AlertState.SetActiveAsync(region.Id, activeNames.Contains(region.Name));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            await Task.Delay(PollInterval, stoppingToken);
        }
    }
}
