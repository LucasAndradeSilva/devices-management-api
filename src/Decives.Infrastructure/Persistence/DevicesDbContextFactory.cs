using Decives.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Decives.Infrastructure.Persistence;

public class DevicesDbContextFactory : IDesignTimeDbContextFactory<DevicesDbContext>
{
    public DevicesDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
        .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../Devices.Api"))
        .AddJsonFile("appsettings.json")
        .Build(); 
        
        var services = new ServiceCollection();
        services.AddInfrastructure(configuration);

        var provider = services.BuildServiceProvider();       
        return provider.GetRequiredService<DevicesDbContext>();
    }
}