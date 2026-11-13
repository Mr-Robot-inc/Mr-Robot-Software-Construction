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
}