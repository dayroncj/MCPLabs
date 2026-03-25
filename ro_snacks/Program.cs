using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ro_snacks.Resources;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

builder.Logging.ClearProviders();

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithResourcesFromAssembly();

var app = builder.Build();

await app.RunAsync();
