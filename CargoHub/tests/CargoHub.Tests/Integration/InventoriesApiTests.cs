using System.Net;
using System.Net.Http.Json;
using CargoHub.Api.Data;
using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace CargoHub.Tests.Integration;

public class InventoriesApiTests : IClassFixture<CargohubApiFactory>
{
    private readonly CargohubApiFactory _factory;
    private readonly HttpClient _client;

    public InventoriesApiTests(CargohubApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<(int itemId, int locationId)> SeedItemAndLocationAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var item = new Item 
        {
            Code = "P01",
            Description = "test",
            BarCode = "9812743897124",
            ModeNumber = "PJ-123",
            CommodityCode = 1234,
            UnitWeight = 12.5f,
            MinPurchaseQty = 1,
            CaseSize = 10,
            PackagingType = "bOX",
            OrderMultiple = 1,
            SupplierSku = "SUP-01",
            SupplierId = 1,
            ItemGroupId = 1,
            ItemLineId = 1,
            ItemTypeId = 1
        };
        var warehouse = new Warehouse
        {
            Code = "test",
            Name = "asd4",
            Address = "boelwvar 12",
            City = "City of hope",
            ZipCode = "1942-12",
            Province = "big province",
            Country = "no mands land",
            ContactName = "abdul",
            ContactPhone = "samsung",
            ContactEmail = "abudl@kraka.pi",
        };
        var location = new Location 
        { 
            Warehouse = warehouse,
            Code = "sqdwd",
            Name = "No name"
        };

        db.Add(item);
        db.Add(location);
        await db.SaveChangesAsync();

        return (item.Id, location.Id);
    }

    [Fact]
    public async Task Create_ValidInventory_Returns201_AndIsSavedInDatabase()
    {
        var (itemId, locationId) = await SeedItemAndLocationAsync();
        var dto = new CreateInventoryDto 
        { 
            ItemId = itemId,
            LocationId = locationId,
            QuantityOnHand = 5,
            QuantityOrdered = 8, 
            QuantityAllocated = 10,
        };

        var response = await _client.PostAsJsonAsync("/api/inventories", dto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var created = await response.Content.ReadFromJsonAsync<InventoryDto>();
        Assert.NotNull(created);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var saved = await db.Inventories.FindAsync(created.Id);
        Assert.NotNull(saved);
        Assert.Equal(itemId, saved.ItemId);
        Assert.Equal(locationId, saved.LocationId);
        Assert.Equal(5, saved.QuantityOnHand);
        Assert.Equal(8, saved.QuantityOrdered);
        Assert.Equal(10, saved.QuantityAllocated);
    }

    [Fact]
    public async Task GetById_UnknownId_Returns404()
    {
        var response = await _client.GetAsync("/api/inventories/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_UnknownId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/inventories/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_UnknownItem_Returns400()
    {
        var (_, locationId) = await SeedItemAndLocationAsync();
        var dto = new CreateInventoryDto 
        { 
            ItemId = 999999,
            LocationId = locationId,
            QuantityOnHand = 5,
            QuantityOrdered = 7, 
            QuantityAllocated = 10,
        };

        var response = await _client.PostAsJsonAsync("/api/inventories", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}