using CargoHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CargoHub.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


    public DbSet<Supplier> Suppliers => Set<Supplier>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Location>()
            .HasOne(l => l.Warehouse)
            .WithMany(w => w.Locations)
            .HasForeignKey(l => l.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Inventory>()
            .HasOne(i => i.Item)
            .WithMany(it => it.Inventories)
            .HasForeignKey(i => i.ItemId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<Inventory>()
            .HasOne(i => i.Location)
            .WithMany()
            .HasForeignKey(i => i.LocationId)
            .OnDelete(DeleteBehavior.Restrict);
    }
    
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Inventory> Inventories => Set<Inventory>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<TransferItem> TransferItems => Set<TransferItem>();
    public DbSet<Location> Locations => Set<Location>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<Warehouse> Warehouses => Set<Warehouse>();
    public DbSet<ItemType> ItemTypes => Set<ItemType>();
}
