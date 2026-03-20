using Decives.Infrastructure.DependencyInjection;
using Decives.Infrastructure.Persistence;
using Devices.Api.Extensions;
using Devices.Api.Middlewares;
using Devices.Application.Interfaces;
using Devices.Application.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Devices.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Controllers
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        // Extensions
        builder.Services.AddApiVersioningConfig();
        builder.Services.AddSwaggerConfig();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddScoped<IDeviceService, DeviceService>();
        builder.Services.AddHealthChecks();
        builder.Services.AddJwtAuth(builder.Configuration);

        builder.AddLoggingConfig();

        var app = builder.Build();

        // Middlewares
        app.UseSerilogRequestLogging();
        app.UseMiddleware<ExceptionMiddleware>();

        // Auth
        app.UseAuthentication();
        app.UseAuthorization();

        // Swagger
        app.UseSwaggerConfig();

        app.MapControllers();
        app.MapHealthChecks("/health");

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DevicesDbContext>();
            db.Database.Migrate();
        }

        app.Run();
    }
}