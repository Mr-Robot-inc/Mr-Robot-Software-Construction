using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Services;
using CargoHub.Tests.Unit.Fakes;

namespace CargoHub.Tests.Unit.Services;

public class InventoryServiceTests
{
    private readonly FakeRepository<Inventory> _repo;
    private readonly InventoryService _service;

    public InventoryServiceTests()
    {
        _repo = new FakeRepository<Inventory>();
        _service = new InventoryService(_repo);
    }

    [Fact]
    public async Task Create_SetsAllFields_AndSaves()
    {
        var dto = new CreateInventoryDto
        {
            ItemId = 1,
            LocationId = 2,
            QuantityOnHand = 3,
            QuantityOrdered = 4,
            QuantityAllocated = 5
        };

        var result = await _service.CreateAsync(dto);

        Assert.Equal(1, result.ItemId);
        Assert.Equal(2, result.LocationId);
        Assert.Equal(3, result.QuantityOnHand);
        Assert.Equal(4, result.QuantityOrdered);
        Assert.Equal(5, result.QuantityAllocated);
        Assert.True(result.Id > 0);
        Assert.Single(_repo.Items);
        Assert.Equal(1, _repo.SaveCount);
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
        var created = await _service.CreateAsync(
            new CreateInventoryDto
            {
                ItemId = 1,
                LocationId = 2,
                QuantityOnHand = 3,
                QuantityOrdered = 10,
                QuantityAllocated = 20
            }
        );

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
        var created = await _service.CreateAsync(new CreateInventoryDto
        {
            ItemId = 1,
            LocationId = 2,
            QuantityOnHand = 3,
            QuantityOrdered = 10,
            QuantityAllocated = 20
        });

        var result = await _service.DeleteAsync(created.Id);

        Assert.True(result);
        Assert.Empty(_repo.Items);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsNull()
    {
        var result = await _service.DeleteAsync(999);

        Assert.Null(result);
    }
}