using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Programme.Api;

public class PlaceholderHealthCheck : IHealthCheck
{
    // EXAMPLE HEALTH CHECK - REMOVE IF NOT REQUIRED

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
    {
        return Task.FromResult(HealthCheckResult.Healthy("ok"));
    }
}