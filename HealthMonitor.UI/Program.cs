using HealthMonitor.UI.DelegatingHandles;
using Microsoft.Extensions.DependencyInjection.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.TryAddTransient<CustomDelegatingHandler>();

builder.Services.AddHealthChecks();
builder.Services
    .AddHealthChecksUI(setup =>
    {
        setup.UseApiEndpointDelegatingHandler<CustomDelegatingHandler>();
        setup.AddHealthCheckEndpoint("Address API", "http://localhost:5212/health");
        setup.SetEvaluationTimeInSeconds(15);
    })
    .AddInMemoryStorage();

var app = builder.Build();

app
    .UseRouting()
    .UseEndpoints(config => config.MapHealthChecksUI());

app.Run();