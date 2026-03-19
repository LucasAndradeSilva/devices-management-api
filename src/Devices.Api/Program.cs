
using Devices.Api.Middlewares;
using Devices.Application.Interfaces;
using Devices.Application.Services;
using Devices.Infrastructure.DependencyInjection;

namespace Devices.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            // Swagger
            builder.Services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new()
                {
                    Title = "Devices API",
                    Version = "v1",
                    Description = "API for managing devices"
                });
            });

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddScoped<IDeviceService, DeviceService>();

            // Health Check
            builder.Services.AddHealthChecks();

            var app = builder.Build();

            // Swagger
            app.UseSwagger();
            app.UseSwaggerUI();

            // Middleware
            app.UseMiddleware<ExceptionMiddleware>();

            app.MapControllers();
            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}
