using CargoHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CargoHub.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client> ();
    public DbSet<Item> Items => Set<Item>();
    public DbSet<Shipment> Shipments => Set<Shipment>();
    public DbSet<Transfer> Transfers => Set<Transfer>();
    public DbSet<TransferItem> TransferItems => Set<TransferItem>();
}