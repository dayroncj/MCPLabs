using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Readers;
using ModelContextProtocol.Protocol;
using System.Text.Json;

namespace ro_soccer.Tools;

public class ToolsDiscoverer
{
    private readonly OpenApiDocument _openApiDocument;
    private readonly HttpClient _httpClient;

    private readonly Dictionary<string, (string PathTemplate, OperationType Method, IList<OpenApiParameter> Parameters)> _toolMap = new();

    public ToolsDiscoverer(string rawOpenApiSpec, HttpClient httpClient)
    {
        _openApiDocument = new OpenApiStringReader().Read(rawOpenApiSpec, out _);
        _httpClient = httpClient;
    }

    public List<Tool> GetMcpTools()
    {
        var tools = new List<Tool>();

        foreach (var path in _openApiDocument.Paths)
        {
            foreach (var operation in path.Value.Operations)
            {
                string toolName = $"{operation.Key.ToString().ToLower()}_{path.Key.Replace("/", "_").Trim('_')}";
                string description = operation.Value.Summary ?? operation.Value.Description ?? toolName;
                var inputSchema = BuildJsonSchemaForParameters(operation.Value.Parameters);

                _toolMap[toolName] = (path.Key, operation.Key, operation.Value.Parameters);
                tools.Add(new Tool { Name = toolName, Description = description, InputSchema = inputSchema });
            }
        }

        return tools;
    }

    public async Task<string> CallApiEndpointAsync(string toolName, Dictionary<string, object> arguments)
    {
        if (!_toolMap.TryGetValue(toolName, out var info))
            throw new ArgumentException($"Tool '{toolName}' not found.");

        var (pathTemplate, method, parameters) = info;

        // Replace path parameters and collect query params
        var resolvedPath = pathTemplate;
        var queryParams = new List<string>();

        foreach (var param in parameters)
        {
            if (!arguments.TryGetValue(param.Name, out var value))
                continue;

            var strValue = value?.ToString() ?? string.Empty;

            if (param.In == ParameterLocation.Path)
                resolvedPath = resolvedPath.Replace($"{{{param.Name}}}", Uri.EscapeDataString(strValue));
            else if (param.In == ParameterLocation.Query)
                queryParams.Add($"{Uri.EscapeDataString(param.Name)}={Uri.EscapeDataString(strValue)}");
        }

        var url = queryParams.Count > 0
            ? resolvedPath + "?" + string.Join("&", queryParams)
            : resolvedPath;

        var httpMethod = method switch
        {
            OperationType.Post   => HttpMethod.Post,
            OperationType.Put    => HttpMethod.Put,
            OperationType.Patch  => HttpMethod.Patch,
            OperationType.Delete => HttpMethod.Delete,
            _                    => HttpMethod.Get
        };

        var request = new HttpRequestMessage(httpMethod, url);
        var response = await _httpClient.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync();
    }

    private static JsonElement BuildJsonSchemaForParameters(IList<OpenApiParameter> parameters)
    {
        var properties = new Dictionary<string, object>();
        var required = new List<string>();

        foreach (var param in parameters)
        {
            var propType = param.Schema?.Type ?? "string";
            properties[param.Name] = new { type = propType, description = param.Description ?? param.Name };

            if (param.Required)
                required.Add(param.Name);
        }

        return required.Count > 0
            ? JsonSerializer.SerializeToElement(new { type = "object", properties, required })
            : JsonSerializer.SerializeToElement(new { type = "object", properties });
    }
}