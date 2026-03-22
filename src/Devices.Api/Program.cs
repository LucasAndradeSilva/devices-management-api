using Decives.Infrastructure.DependencyInjection;
using Decives.Infrastructure.Persistence;
using Devices.Api.Extensions;
using Devices.Api.Middlewares;
using Devices.Application.Interfaces;
using Devices.Application.Services;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Data.Entity;

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