namespace AlertsApi.Models;

public record AlertsInUaResponse(List<AlertsInUaAlert> Alerts);

public record AlertsInUaAlert(
    int Id,
    string LocationTitle,
    string LocationType,
    string LocationOblast,
    string AlertType,
    string AlertLevel,
    DateTimeOffset StartedAt,
    DateTimeOffset? FinishedAt,
    string? LocationRaion,
    List<AlertsInUaThreat>? Threats
);

public record AlertsInUaThreat(
    string ThreatType,
    string Level,
    DateTimeOffset StartedAt
);
