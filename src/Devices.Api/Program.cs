using Decives.Infrastructure.DependencyInjection;
using Decives.Infrastructure.Persistence;
using Devices.Api.Extensions;
using Devices.Api.Middlewares;
using Devices.Application.Interfaces;
using Devices.Application.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data.Entity;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

namespace Devices.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Controllers
        builder.Services.AddControllers()
          .AddJsonOptions(options =>
           {
               options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
           });

        builder.Services.AddEndpointsApiExplorer();

        // Extensions
        builder.Services.AddApiVersioningConfig();
        builder.Services.AddSwaggerConfig();
        builder.Services.AddInfrastructure(builder.Configuration);
        builder.Services.AddScoped<IDeviceService, DeviceService>();
        builder.Services.AddHealthChecks();
        builder.Services.AddJwtAuth(builder.Configuration);

        builder.Services.AddRateLimiter(options =>
        {
            options.AddPolicy("fixed", context =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "global",
                    factory: _ => new FixedWindowRateLimiterOptions
                    {
                        PermitLimit = 10, // 10 requests
                        Window = TimeSpan.FromSeconds(10), // a cada 10s
                        QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                        QueueLimit = 2
                    }));
        });

        builder.AddLoggingConfig();

        var app = builder.Build();

        // Middlewares
        app.UseSerilogRequestLogging();
        app.UseMiddleware<ExceptionMiddleware>();
        app.UseRateLimiter();

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

            if (db.Database.IsRelational())
            {
                Console.WriteLine("Applying migrations...");
                var retries = 0;
                while (retries < 10)
                {
                    try
                    {

                        db.Database.Migrate();
                        break;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Attempt {retries}");

                        Console.WriteLine(ex.Message);

                        retries++;
                        Thread.Sleep(5000);
                    }
                }
            }
        }

        Console.WriteLine("API Started in Enviroment: " + builder.Environment.EnvironmentName);

        app.MapGet("/", () => "OK");

        app.Run();
    }
}