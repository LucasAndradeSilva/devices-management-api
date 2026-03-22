using Decives.Infrastructure.Persistence;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Devices.IntegrationTest;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {            
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<DevicesDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);
            
            services.AddDbContext<DevicesDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });
        });
    }
}