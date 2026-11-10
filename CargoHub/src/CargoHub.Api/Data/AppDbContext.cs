using CargoHub.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace CargoHub.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Client> Clients => Set<Client> ();
<<<<<<< HEAD
    public DbSet<Warehouse> Warehouses => Set<Warehouse> ();
=======
    public DbSet<Item> Items => Set<Item> ();
>>>>>>> origin/main
}