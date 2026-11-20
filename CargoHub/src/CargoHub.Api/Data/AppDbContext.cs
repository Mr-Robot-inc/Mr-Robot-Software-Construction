using CargoHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CargoHub.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    //Automatic time changes
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State == EntityState.Added)
                entry.Entity.CreatedAt = now;
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
                entry.Entity.UpdatedAt = now;
        }
        return base.SaveChangesAsync(cancellationToken);
    }

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

        modelBuilder.Entity<Item>()
            .HasOne(i => i.ItemGroup)                  
            .WithMany(g => g.Items)                    
            .HasForeignKey(i => i.ItemGroupId)         
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Item>()
            .HasOne(i => i.ItemLine)                  
            .WithMany(l => l.Items)                    
            .HasForeignKey(i => i.ItemLineId)         
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Item>()
            .HasOne(i => i.ItemType)                  
            .WithMany(t => t.Items)                    
            .HasForeignKey(i => i.ItemTypeId)         
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
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<ItemType> ItemTypes => Set<ItemType>();
    public DbSet<ItemLine> ItemLines => Set<ItemLine>();
    public DbSet<ItemGroup> ItemGroups => Set<ItemGroup>();
}
