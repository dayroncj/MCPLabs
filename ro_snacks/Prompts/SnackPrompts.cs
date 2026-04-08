// using Microsoft.Extensions.AI;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using System.ComponentModel;

namespace ro_snacks.Prompts;

[McpServerPromptType]
public class SnacksPrompts
{
    /// <summary>
    /// Prompt para asistir al usuario en el agendamiento de snacks, para un día en particular.
    /// </summary>
    [McpServerPrompt(Name = "agendamiento_asistido")]
    [Description("Guía al usuario a agendar su próximo snack.")]
    public static IEnumerable<PromptMessage> BookSnack(
        [Description("Nombre del usuario.")] string user,
        [Description("Fecha de inicio (formato yyyy-MM-dd).")] string date,
        [Description("Snacks que no gustan al usuario, separados por comas.")] string dislikedSnacks
        )
    {
        return
        [
            new PromptMessage
            {
                Role = Role.User,
                Content = new TextContentBlock
                {
                    Text = $"""
                        Eres un asistente para agendamiento de snacks.
                        
                        Tu tarea es:
                        1. Consultar el menú semanal usando el recurso disponible
                        2. Revisar agendamientos existentes con GetSnacksSchedule
                        3. Proponer el próximo snack a agendar, teniendo en cuenta los gustos del usuario y el menú disponible para cada día 
                        4. No se deben proponer snacks que el usuario haya agendado en las últimas 2 semanas
                        5. Usar AddSnackToSchedule para registrar cada snack aprobado
                        
                        Usuario: {user}
                        Fecha: {date}
                        Snacks no deseados: {dislikedSnacks}

                        1. Mostrar las opciones para el día seleccionado
                        2. Indicar si ya había un snack agendado
                        3. Agendar el snack propuesto si aún no se ha agendado
                        """
                }
            }
        ];
    }
}