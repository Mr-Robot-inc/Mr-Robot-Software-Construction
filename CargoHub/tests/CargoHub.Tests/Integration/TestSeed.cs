using CargoHub.Api.Data;
using CargoHub.Api.Models;

namespace CargoHub.Tests.Integration;

internal static class TestSeed
{
    public static async Task<int> ClientAsync(AppDbContext db)
    {
        var client = new Client
        {
            Name = "Client " + Guid.NewGuid().ToString("N")[..6],
            Address = "Street 1",
            City = "City",
            ZipCode = "1234AB",
            Province = "Province",
            Country = "Country",
            ContactName = "Bob",
            ContactPhone = "0612345678",
            ContactEmail = "bob@example.com"
        };
        db.Add(client);
        await db.SaveChangesAsync();
        return client.Id;
    }

    public static async Task<int> WarehouseAsync(AppDbContext db)
    {
        var warehouse = new Warehouse
        {
            Code = "WH" + Guid.NewGuid().ToString("N")[..6],
            Name = "Warehouse",
            Address = "Street 1",
            City = "City",
            ZipCode = "1234AB",
            Province = "Province",
            Country = "Country",
            ContactName = "Bob",
            ContactPhone = "0612345678",
            ContactEmail = "bob@example.com"
        };
        db.Add(warehouse);
        await db.SaveChangesAsync();
        return warehouse.Id;
    }

    public static async Task<int> ItemAsync(AppDbContext db)
    {
        var item = new Item
        {
            Code = "P" + Guid.NewGuid().ToString("N")[..6],
            Description = "test",
            BarCode = "9812743897124",
            ModeNumber = "PJ-123",
            CommodityCode = 1234,
            UnitWeight = 12.5f,
            MinPurchaseQty = 1,
            CaseSize = 10,
            PackagingType = "Box",
            OrderMultiple = 1,
            SupplierSku = "SUP-01",
            SupplierId = 1,
            ItemGroup = new ItemGroup { Name = "Tools", Description = "Hand tools", ItemGroupcol = "p-231" },
            ItemLine = new ItemLine { Name = "line", Description = "line descr" },
            ItemType = new ItemType { Name = "type", Description = "type descr" }
        };
        db.Add(item);
        await db.SaveChangesAsync();
        return item.Id;
    }

    public static async Task InventoryAsync(AppDbContext db, int itemId, int quantityOnHand, int quantityAllocated)
    {
        var location = new Location
        {
            Code = "LOC" + Guid.NewGuid().ToString("N")[..6],
            Name = "Location",
            WarehouseId = await WarehouseAsync(db)
        };
        db.Add(location);
        await db.SaveChangesAsync();

        db.Add(new Inventory
        {
            ItemId = itemId,
            LocationId = location.Id,
            QuantityOnHand = quantityOnHand,
            QuantityAllocated = quantityAllocated
        });
        await db.SaveChangesAsync();
    }

    public static async Task<int> OrderAsync(AppDbContext db, int? clientId = null, int? warehouseId = null)
    {
        var cid = clientId ?? await ClientAsync(db);
        var wid = warehouseId ?? await WarehouseAsync(db);

        var order = new Order
        {
            ClientId = cid,
            OrderDate = new DateTime(2026, 11, 1, 0, 0, 0, DateTimeKind.Utc),
            RequestDate = new DateTime(2026, 11, 5, 0, 0, 0, DateTimeKind.Utc),
            Reference = "ORD-TEST",
            OrderStatus = "Pending",
            WarehouseId = wid,
            ShipToClientId = cid,
            BillToClientId = cid
        };
        db.Add(order);
        await db.SaveChangesAsync();
        return order.Id;
    }
}
