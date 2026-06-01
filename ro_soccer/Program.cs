using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Protocol;
using ro_soccer.Tools;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Logging.ClearProviders();

var httpClient = new HttpClient();
httpClient.BaseAddress = new Uri("https://api.football-data.org");
httpClient.DefaultRequestHeaders.Add("X-Auth-Token", "mi_token");

var discoverer = new ToolsDiscoverer(File.ReadAllText("football_data_openapi.yml"), httpClient);
var tools = discoverer.GetMcpTools();

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithResourcesFromAssembly()
    .WithPromptsFromAssembly() // Responde con los tools descubiertos cuando el cliente MCP ejecute tools/list
    .WithListToolsHandler(async (request, ct) => // Delega a callApiEndpointAsync cuando el cliente MCP ejecute tools/call
    {
        return new ListToolsResult { Tools = tools };
    })
    .WithCallToolHandler(async (request, ct) =>
    {
        var toolName = request.Params!.Name;

        var args = request.Params.Arguments?
            .ToDictionary(kvp => kvp.Key, kvp => (object)kvp.Value.ToString()!)
            ?? new Dictionary<string, object>();

        var result = await discoverer.CallApiEndpointAsync(toolName, args);

        return new CallToolResult
        {
            Content = [new TextContentBlock { Text = result }]
        };
    });

var app = builder.Build();

await app.RunAsync();

