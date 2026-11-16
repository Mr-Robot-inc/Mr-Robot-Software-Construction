using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Services;
using CargoHub.Tests.Unit.Fakes;

namespace CargoHub.Tests.Unit.Services;

public class InventoryServiceTests
{
    private readonly FakeRepository<Inventory> _repo;
    private readonly FakeRepository<Item> _items;
    private readonly FakeRepository<Location> _locations;
    private readonly InventoryService _service;


    public InventoryServiceTests()
    {
        _repo = new FakeRepository<Inventory>();
        _items = new FakeRepository<Item>();
        _locations = new FakeRepository<Location>();
        _service = new InventoryService(_repo, _items, _locations);
    }

    private (int itemId, int locationId) SeedItemsAndLocations()
    {
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
        var location = new Location 
        { 
            WarehouseId = 10,
            Code = "sqdwd",
            Name = "No name"
        };

        _items.Add(item);
        _locations.Add(location);
        return (item.Id, location.Id);

    }

    [Fact]
    public async Task Create_SetsAllFields_AndSaves()
    {
        var (itemId, locationId) = SeedItemsAndLocations();
        var dto = new CreateInventoryDto
        {
            ItemId = itemId,
            LocationId = locationId,
            QuantityOnHand = 3,
            QuantityOrdered = 4,
            QuantityAllocated = 5
        };

        var result = await _service.CreateAsync(dto);

        Assert.Equal(itemId, result.ItemId);
        Assert.Equal(locationId, result.LocationId);
        Assert.Equal(3, result.QuantityOnHand);
        Assert.Equal(4, result.QuantityOrdered);
        Assert.Equal(5, result.QuantityAllocated);
        Assert.True(result.Id > 0);
        Assert.Single(_repo.Items);
        Assert.Equal(1, _repo.SaveCount);
    }

    [Fact]
    public async Task Create_UnknownItem_ReturnsNull()
    {
        var (_, locationId) = SeedItemsAndLocations();
        var dto = new CreateInventoryDto 
        { 
            ItemId = 999,
            LocationId = locationId,
            QuantityOnHand = 3,
            QuantityOrdered = 4,
            QuantityAllocated = 5
        };

        var result = await _service.CreateAsync(dto);

        Assert.Null(result);
        Assert.Empty(_repo.Items);
        Assert.Equal(0, _repo.SaveCount);
    }

    [Fact]
    public async Task GetById_UnknownId_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(999);

        Assert.Null(result);
    }

    [Fact]
    public async Task Update_ExistingId_ChangesFields()
    {
        var (itemId, locationId) = SeedItemsAndLocations();

        var created = await _service.CreateAsync(
            new CreateInventoryDto
            {
                ItemId = itemId,
                LocationId = locationId,
                QuantityOnHand = 3,
                QuantityOrdered = 10,
                QuantityAllocated = 20
            }
        );

        Assert.NotNull(created);

        var update = new UpdateInventoryDto
        {
            ItemId = 3,
            LocationId = 4,
            QuantityOnHand = 5,
            QuantityOrdered = 20,
            QuantityAllocated = 30
        };

        var result = await _service.UpdateAsync(created.Id, update);

        Assert.NotNull(result);
        Assert.Equal(3, result.ItemId);
        Assert.Equal(4, result.LocationId);
        Assert.Equal(5, result.QuantityOnHand);
        Assert.Equal(20, result.QuantityOrdered);
        Assert.Equal(30, result.QuantityAllocated);
        Assert.Equal(2, _repo.SaveCount);
    }

    [Fact]
    public async Task Update_UnknownId_ReturnsNull()
    {
        var update = new UpdateInventoryDto
        {
            ItemId = 3,
            LocationId = 4,
            QuantityOnHand = 5,
            QuantityOrdered = 20,
            QuantityAllocated = 30
        };

        var result = await _service.UpdateAsync(999, update);

        Assert.Null(result);
        Assert.Equal(0, _repo.SaveCount);
    }

    [Fact]
    public async Task Delete_ExistingId_RemovesItem()
    {
        var (itemId, locationId) = SeedItemsAndLocations();

        var created = await _service.CreateAsync(new CreateInventoryDto
        {
            ItemId = itemId,
            LocationId = locationId,
            QuantityOnHand = 3,
            QuantityOrdered = 10,
            QuantityAllocated = 20
        });

        Assert.NotNull(created);

        var result = await _service.DeleteAsync(created.Id);

        Assert.True(result);
        Assert.Empty(_repo.Items);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsFalse()
    {
        var result = await _service.DeleteAsync(999);

        Assert.False(result);
    }
}