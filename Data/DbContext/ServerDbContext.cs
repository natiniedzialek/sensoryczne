using HttpServer.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HttpServer.Data.DbContext;

public class ServerDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public ServerDbContext()
    {
    }
    
    public ServerDbContext(DbContextOptions<ServerDbContext> options) : base(options)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>()
            .HasMany(u => u.Devices)
            .WithMany(d => d.Users)
            .UsingEntity<Dictionary<string, object>>(
                "UserDevice",
                j => j.HasOne<Device>().WithMany().HasForeignKey("DeviceId"),
                j => j.HasOne<User>().WithMany().HasForeignKey("UserId"));

        modelBuilder.Entity<Device>()
            .HasMany(d => d.Measurements)
            .WithOne(m => m.Device)
            .HasForeignKey(m => m.DeviceId);
    }

    public async Task ConnectDatabase()
    {
        if (!await Database.CanConnectAsync())
        {
            Console.WriteLine("Database disconnected. Trying to reconnect...");
            await Database.OpenConnectionAsync();
        }
    }

    public DbSet<Device> Devices { get; set; }
    public DbSet<Measurement> Measurements { get; set; }
    public DbSet<User> Users { get; set; }
}