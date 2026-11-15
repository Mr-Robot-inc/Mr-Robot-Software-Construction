
using CargoHub.Api.Data;
using CargoHub.Api.Models;

namespace CargoHub.Api.Repositories;

public class DataInporter
{
    public static async Task RunImportAsync(
        AppDbContext context,
        string contentRootPath)
    {
        var importer = new JsonDataImporter(context);

        var jsonPath = Path.Combine(contentRootPath, "Json");

        // Suppliers must be imported before items.
        var suppliers = await importer.ImportAsync<Supplier>(
            Path.Combine(jsonPath, "supplier.json"));

        var clients = await importer.ImportAsync<Client>(
            Path.Combine(jsonPath, "client.json"));

        var warehouses = await importer.ImportAsync<Warehouse>(
            Path.Combine(jsonPath, "warehouse.json"));

        var items = await importer.ImportAsync<Item>(
            Path.Combine(jsonPath, "item.json"));

        Console.WriteLine($"Suppliers imported: {suppliers}");
        Console.WriteLine($"Clients imported: {clients}");
        Console.WriteLine($"Warehouses imported: {warehouses}");
        Console.WriteLine($"Items imported: {items}");
    }
}
