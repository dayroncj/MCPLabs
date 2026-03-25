using ModelContextProtocol;
using ModelContextProtocol.Server;
using System.ComponentModel;
using System.Text.Json;

namespace ro_snacks.Resources;

[McpServerResourceType]
public class SnacksResources
{
    [McpServerResource(Name = "Menú de Snacks", Title = "Menú de snacks title", MimeType = "application/json")]
    [Description("Retorna el menú semanal de snacks.")]
    public string GetSnacksMenu()
    {
        var menu = new[]
        {
            new { day = "monday",    fruits = new[] { "banano", "fresa", "manzana" } },
            new { day = "tuesday",   fruits = new[] { "mango", "uva", "pera" } },
            new { day = "wednesday", fruits = new[] { "papaya", "kiwi", "sandía" } },
            new { day = "thursday",  fruits = new[] { "piña", "durazno", "naranja" } },
            new { day = "friday",    fruits = new[] { "pera", "mandarina", "manzana" } }
        };

        // Calorías promedio por porción (~100g) de cada fruta
        var calorias = new Dictionary<string, int>
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

        var resultMenu = menu.Select(item => new
        {
            item.day,
            fruits = item.fruits.Select(fruit =>
            {
                return calorias.TryGetValue(fruit, out var cal) ? $"{fruit} ({cal} cal)" : fruit;
            }).ToArray()
        }).ToArray();

        return JsonSerializer.Serialize(resultMenu);
    }
}