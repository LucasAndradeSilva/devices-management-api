using Devices.Api.Middlewares;
using Serilog;

namespace Devices.Api.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseAppMiddleware(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging();

        app.UseMiddleware<ExceptionMiddleware>();

        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();

        return app;
    }
}