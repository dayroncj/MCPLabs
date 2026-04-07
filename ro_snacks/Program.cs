using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Http;
using ro_snacks.Resources;
using ro_snacks.Tools;

var builder = Host.CreateEmptyApplicationBuilder(settings: null);

// builder.Services.AddHttpClient<SnackTools>(client =>
// {
//     client.BaseAddress = new Uri("http://localhost:5076");
// });

builder.Logging.ClearProviders();

builder.Services.AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly()
    .WithResourcesFromAssembly();

var app = builder.Build();

await app.RunAsync();
