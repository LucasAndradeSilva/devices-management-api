using Decives.Infrastructure.DependencyInjection;
using Decives.Infrastructure.Persistence;
using Devices.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Devices.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Services
        builder.Services
            .AddApiConfig()
            .AddApiVersioningConfig()
            .AddSwaggerConfig()
            .AddInfrastructure(builder.Configuration)
            .AddJwtAuth(builder.Configuration)
            .AddRateLimiting()
            .AddHealthCheckConfig();

        builder.AddLoggingConfig();

        var app = builder.Build();

        // Middleware
        app.UseAppMiddleware();

        // Swagger
        app.UseSwaggerConfig();

        // Endpoints
        app.MapControllers();
        app.MapHealthChecks("/health");

        // Migration
        ApplyMigrations(app);

        app.Run();
    }

    private static void ApplyMigrations(WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<DevicesDbContext>();

        if (db.Database.IsRelational())
        {
            Console.WriteLine("Applying migrations...");

            var retries = 0;

            while (retries < 10)
            {
                try
                {
                    db.Database.Migrate();
                    Console.WriteLine("Migrations applied successfully.");
                    break;
                }
                catch (Exception ex)
                {
                    retries++;
                    Console.WriteLine($"Attempt {retries}: {ex.Message}");
                    Thread.Sleep(5000);
                }
            }
        }

        Console.WriteLine($"API Started in Environment: {app.Environment.EnvironmentName}");
    }
}