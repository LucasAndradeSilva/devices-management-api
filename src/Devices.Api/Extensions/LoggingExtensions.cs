using Serilog;

namespace Devices.Api.Extensions;

public static class LoggingExtensions
{
    public static void AddLoggingConfig(this WebApplicationBuilder builder)
    {
        Log.Logger = new LoggerConfiguration()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        builder.Host.UseSerilog((ctx, lc) =>
            lc.Enrich.FromLogContext()
              .Enrich.WithCorrelationId()
              .ReadFrom.Configuration(ctx.Configuration));
    }
}