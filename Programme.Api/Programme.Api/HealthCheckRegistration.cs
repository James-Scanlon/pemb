using Microsoft.Extensions.DependencyInjection;

namespace Programme.Api;

public static class HealthCheckRegistration
{
    public static void ConfigureHealthChecks(this IServiceCollection serviceCollection)
    {
        // REGISTER ANY HEALTH CHECKS HERE - THEY WILL BE CALLED WHEN HITTING THE /healthz ENDPOINT

        serviceCollection.AddHealthChecks()
            .AddCheck<PlaceholderHealthCheck>("placeholder");
    }
}