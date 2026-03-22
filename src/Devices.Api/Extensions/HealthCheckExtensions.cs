namespace Devices.Api.Extensions;

public static class HealthCheckExtensions
{
    public static IServiceCollection AddHealthCheckConfig(this IServiceCollection services)
    {
        services.AddHealthChecks();
        return services;
    }
}