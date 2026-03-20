
using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Devices.Api.Middlewares;
using Devices.Application.Interfaces;
using Devices.Application.Services;
using Devices.Infrastructure.DependencyInjection;
using Serilog;

namespace Devices.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();          

            // Version
            builder.Services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
                
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            })
            .AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV"; // v1, v1.0
                options.SubstituteApiVersionInUrl = true;
            });

            // Swagger
            var provider = builder.Services.BuildServiceProvider()
            .GetRequiredService<IApiVersionDescriptionProvider>();

            builder.Services.AddSwaggerGen(options =>
            {
                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerDoc(description.GroupName, new()
                    {
                        Title = $"Devices API {description.ApiVersion}",
                        Version = description.ApiVersion.ToString()
                    });
                }
            });

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddScoped<IDeviceService, DeviceService>();

            // Health Check
            builder.Services.AddHealthChecks();

            // Logs
            Log.Logger = new LoggerConfiguration()
              .Enrich.FromLogContext()              
              .WriteTo.Console()
              .WriteTo.File(
                  "logs/log-.txt",
                  rollingInterval: RollingInterval.Day)
              .CreateLogger();

            builder.Host.UseSerilog((ctx, lc) =>
                lc.Enrich.FromLogContext()
                  .Enrich.WithCorrelationId()
                  .ReadFrom.Configuration(ctx.Configuration));

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            // Swagger
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

                foreach (var description in provider.ApiVersionDescriptions)
                {
                    options.SwaggerEndpoint(
                        $"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName.ToUpperInvariant());
                }
            });

            // Middleware
            app.UseMiddleware<ExceptionMiddleware>();

            app.MapControllers();
            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}
