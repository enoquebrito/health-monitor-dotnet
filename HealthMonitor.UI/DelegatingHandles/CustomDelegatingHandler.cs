using System.Diagnostics;
using System.Text;
using System.Text.Json;
using HealthChecks.UI.Core;

namespace HealthMonitor.UI.DelegatingHandles;

public class CustomDelegatingHandler(ILogger<CustomDelegatingHandler> logger) : DelegatingHandler
{
    private readonly ILogger<CustomDelegatingHandler> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var sw = Stopwatch.StartNew();
        
        var response = await base.SendAsync(request, cancellationToken);

        sw.Stop();
        
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        var report = new Dictionary<string, UIHealthReportEntry>
        {
            { "API", new UIHealthReportEntry { Status = Enum.Parse<UIHealthStatus>(content) } }
        };

        var parsedContent = new UIHealthReport(report, sw.Elapsed);

        response.Content =
            new StringContent(JsonSerializer.Serialize(parsedContent), Encoding.UTF8, "application/json");
        
        _logger.LogInformation("Response Content: {Content}", content);

        return response;
    }
}