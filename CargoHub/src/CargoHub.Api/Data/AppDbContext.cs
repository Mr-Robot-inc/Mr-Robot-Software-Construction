using CargoHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CargoHub.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client> ();
<<<<<<< HEAD
    public DbSet<Warehouse> Warehouses => Set<Warehouse> ();
    public DbSet<Location> Locations => Set<Location> ();


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Location>()
            .HasOne(l => l.Warehouse)
            .WithMany(w => w.Locations)
            .HasForeignKey(l => l.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

    
=======
    public DbSet<Item> Items => Set<Item> ();
}
>>>>>>> main
