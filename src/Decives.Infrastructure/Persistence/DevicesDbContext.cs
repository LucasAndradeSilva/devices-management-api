using Devices.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Devices.Infrastructure.Persistence;

public class DevicesDbContext : DbContext
{
    public DevicesDbContext(DbContextOptions<DevicesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Device> Devices { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(d => d.Id);

            entity.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(d => d.Brand)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(d => d.State)
                .IsRequired();

            entity.Property(d => d.CreatedAt)
                .IsRequired();
        });
    }
}