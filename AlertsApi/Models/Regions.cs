namespace AlertsApi.Models;

public record Region(string Id, string Name);

public static class Regions
{
    public static readonly IReadOnlyList<Region> All =
    [
        new("3", "Хмельницька область"),
        new("4", "Вінницька область"),
        new("5", "Рівненська область"),
        new("8", "Волинська область"),
        new("9", "Дніпропетровська область"),
        new("10", "Житомирська область"),
        new("11", "Закарпатська область"),
        new("12", "Запорізька область"),
        new("13", "Івано-Франківська область"),
        new("14", "Київська область"),
        new("15", "Кіровоградська область"),
        new("16", "Луганська область"),
        new("17", "Миколаївська область"),
        new("18", "Одеська область"),
        new("19", "Полтавська область"),
        new("20", "Сумська область"),
        new("21", "Тернопільська область"),
        new("22", "Харківська область"),
        new("23", "Херсонська область"),
        new("24", "Черкаська область"),
        new("25", "Чернігівська область"),
        new("26", "Чернівецька область"),
        new("27", "Львівська область"),
        new("28", "Донецька область"),
        new("29", "Автономна Республіка Крим"),
        new("30", "м. Севастополь"),
        new("31", "м. Київ"),
    ];

    private static readonly Dictionary<string, Region> ById = All.ToDictionary(r => r.Id);
    private static readonly Dictionary<string, Region> ByName = All.ToDictionary(r => r.Name);

    public static Region? FindById(string id) => ById.GetValueOrDefault(id);
    public static Region? FindByName(string name) => ByName.GetValueOrDefault(name);
}
