using ModelContextProtocol.Server;
using ro_snacks.Resources;
using System.ComponentModel;
using System.Globalization;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace ro_snacks.Tools;

[McpServerToolType]
public class SnackTools
{
    private readonly HttpClient _client;

    public SnackTools(/*HttpClient client*/)
    {
        // _client = client;

        var client = new HttpClient();
        client.BaseAddress = new Uri("http://localhost:5076");
        _client = client;
    }

    [McpServerTool, Description("Obtiene los agendamientos de snack para la semana.")]
    public async Task<List<SnackSchedule>> GetSnacksSchedule(
        [Description("La fecha a partir de la cual se desea conocer el agendamiento de snacks. Formato: yyyy-MM-dd")] string date)
    {
        Console.Error.WriteLine($"Obteniendo agendamiento de snacks para la fecha: {date}");

        if (!DateOnly.TryParse(date, out var _))
        {
            throw new ArgumentException("La fecha proporcionada no es válida. Asegúrate de usar el formato yyyy-MM-dd.");
        }

        try
        {
            var response = await _client.GetAsync($"/snacksSchedule?date={date}");
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<SnackSchedule>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? [];
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            throw new Exception($"Error al obtener el agendamiento de snacks: {ex.Message}");
        }
    }

    [McpServerTool, Description("Agrega un nuevo agendamiento de snack para un día específico.")]
    public async Task<string> AddSnackToSchedule(
        [Description("La fecha en la que se desea agendar el snack. Formato: yyyy-MM-dd")] string date,
        [Description("El usuario que agenda el snack.")] string user,
        [Description("El snack a agendar.")] string snack)
    {
        try
        {
            var allowedSnacks = SnacksResources.GetAllowedSnacks()
            .Select(NormalizeText)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

            if (!allowedSnacks.Contains(NormalizeText(snack)))
            {
                throw new ArgumentException($"El snack '{snack}' no está en el menú permitido.");
            }

            var newSnack = new { Date = date, User = user, Snack = snack };
            var response = await _client.PostAsJsonAsync("/snacksSchedule", newSnack);
            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.Message);
            throw new Exception($"Error al agregar el agendamiento de snack: {ex.Message}");
        }
    }

    private static string NormalizeText(string value)
    {
        var normalized = value.Trim().ToLowerInvariant().Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder(normalized.Length);

        foreach (var ch in normalized)
        {
            if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(ch);
            }
        }

        return builder.ToString().Normalize(NormalizationForm.FormC);
    }

    public record SnackSchedule(DateOnly Date, string User, string Snack);
}