using ModelContextProtocol;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace ro_snacks.Resources;

[McpServerResourceType]
public class SnacksResources
{
    public static readonly IReadOnlyDictionary<string, string[]> WeeklyMenu = new Dictionary<string, string[]>
    {
        { "monday",    ["banano", "fresa", "manzana"] },
        { "tuesday",   ["mango", "uva", "pera"] },
        { "wednesday", ["papaya", "kiwi", "sandía"] },
        { "thursday",  ["piña", "durazno", "naranja"] },
        { "friday",    ["pera", "mandarina", "manzana"] }
    };

    public static readonly IReadOnlyDictionary<string, int> CaloriesByFruit = new Dictionary<string, int>
    {
        { "banano",     89 },
        { "fresa",      32 },
        { "manzana",    52 },
        { "mango",      60 },
        { "uva",        69 },
        { "pera",       57 },
        { "papaya",     43 },
        { "kiwi",       61 },
        { "sandía",     30 },
        { "piña",       50 },
        { "durazno",    39 },
        { "naranja",    47 },
        { "mandarina",  53 }
    };

    public static HashSet<string> GetAllowedSnacks() =>
        WeeklyMenu
            .SelectMany(day => day.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

    [McpServerResource(Name = "Menú de Snacks", Title = "Menú de snacks title", MimeType = "application/json")]
    [Description("Retorna el menú semanal de snacks.")]
    public string GetSnacksMenu()
    {
        var resultMenu = WeeklyMenu.Select(item => new
        {
            day = item.Key,
            fruits = item.Value.Select(fruit =>
            {
                return CaloriesByFruit.TryGetValue(fruit, out var cal) ? $"{fruit} ({cal} cal)" : fruit;
            }).ToArray()
        }).ToArray();

        return JsonSerializer.Serialize(resultMenu);
    }
}