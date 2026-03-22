using System.Text.Json.Serialization;

namespace Devices.Api.Extensions;
public static class ApiConfigExtensions
{
    public static IServiceCollection AddApiConfig(this IServiceCollection services)
    {
        services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters
                    .Add(new JsonStringEnumConverter());
            });

        services.AddEndpointsApiExplorer();

        return services;
    }
}