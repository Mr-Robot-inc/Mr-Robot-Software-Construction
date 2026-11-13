using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;
using CargoHub.Api.Services;

namespace CargoHub.Tests.Services;

public class ShipmentServiceTests
{
    private readonly TestShipmentRepository _repository;
    private readonly ShipmentService _service;

    public ShipmentServiceTests()
    {
        _repository = new TestShipmentRepository();
        _service = new ShipmentService(_repository);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllShipments()
    {
        _repository.Add(CreateShipment(1));
        _repository.Add(CreateShipment(2));

        var result = await _service.GetAllAsync();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsShipmentById()
    {
        var shipment = CreateShipment(1);
        _repository.Add(shipment);

        var result = await _service.GetByIdAsync(1);
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("SHIP001", result.Reference);
    }

    [Fact]
    public async Task GetOrderIdsInShipmentAsync_ReturnsOrderId_WhenShipmentExists()
    {
        var shipment = CreateShipment(1);
        shipment.OrderId = 10;
        _repository.Add(shipment);

        var result = await _service.GetOrderIdsInShipmentAsync(1);
        Assert.Single(result);
        Assert.Equal(10, result[0]);

    }

    [Fact]
    public async Task GetOrderIdsInShipmentAsync_ReturnsOrderId_WhenShipmentDoesNotExist()
    {
        var result = await _service.GetOrderIdsInShipmentAsync(1);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesShipment()
    {
        var dto = new CreateShipmentDto
        {
            Reference = "SHIP003",
            OrderId = 15,
            ShipmentDate = new DateTime(2026, 11, 12),
            ShipmentType = "Fast",
            ShipmentStatus = "Pending",
            CarrierName = "VintedGo",
            ShippingMethod = "Air",
            PaymentType = "DollaBills"
        };

        var result = await _service.CreateAsync(dto);

        Assert.Equal("SHIP003", result.Reference);
        Assert.Equal(1, result.Id);
        Assert.Equal("VintedGo", result.CarrierName);
        Assert.Single(_repository.Shipments);
        Assert.Equal(1, _repository.SaveChangesCount);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingShipment()
    {
        var shipment = CreateShipment(1);
        _repository.Add(shipment);

        var dto = new UpdateShipmentDto
        {
            Reference = "SHIP007",
            OrderId = 9,
            ShipmentDate = new DateTime(2026, 11, 12),
            ShipmentType = "Slow",
            ShipmentStatus = "Shipped",
            CarrierName = "TinkerBell",
            ShippingMethod = "Air",
            PaymentType = "PixieDust"
        };

        var result = await _service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("SHIP007", result.Reference);
        Assert.Equal(9, result.OrderId);
        Assert.Equal("TinkerBell", result.CarrierName);
        Assert.Equal(1, _repository.SaveChangesCount);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNullWhenShipmentDoesNotExist()
    {
        var dto = new UpdateShipmentDto
        {
            Reference = "SHIP007",
            OrderId = 9,
            ShipmentDate = new DateTime(2026, 11, 12),
            ShipmentType = "Normal",
            ShipmentStatus = "Shipped",
            CarrierName = "TinkerBell",
            ShippingMethod = "Air",
            PaymentType = "PixieDust"
        };

        var result = await _service.UpdateAsync(8, dto);

        Assert.Null(result);
        Assert.Equal(0, _repository.SaveChangesCount);
    }

    [Fact]
    public async Task DeleteAsync_DeletesExistingShipment()
    {
        _repository.Add(CreateShipment(1));

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
        Assert.Empty(_repository.Shipments);
        Assert.Equal(1, _repository.SaveChangesCount);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNullWhenShipmentDoesNotExist()
    {
        var result = await _service.DeleteAsync(1);

        Assert.Null(result);
        Assert.Equal(0, _repository.SaveChangesCount);
    }

    private static Shipment CreateShipment(int id)
    {
        return new Shipment
        {
            Id = id,
            Reference = $"SHIP{id:000}",
            OrderId = 1,
            ShipmentDate = new DateTime(2026, 11, 12),
            ShipmentType = "Turbo",
            ShipmentStatus = "Shipped",
            CarrierName = "Santa",
            ShippingMethod = "Air",
            PaymentType = "DollaDollaBills",
            CreatedAt = new DateTime(2026, 11, 11),
            UpdatedAt = new DateTime(2026, 11, 12)
        };
    }

    private class TestShipmentRepository : IRepository<Shipment>
    {
        public List<Shipment> Shipments { get; } = new();
        public int SaveChangesCount { get; private set; }
        public Task<List<Shipment>> GetAllAsync() => Task.FromResult(Shipments.ToList());
        public Task<Shipment?> GetByIdAsync(int id) => Task.FromResult(Shipments.FirstOrDefault(s => s.Id == id));
        public void Add(Shipment entity)
        {
            if (entity.Id == 0)
            {
                entity.Id = Shipments.Count == 0 ? 1 : Shipments.Max(s => s.Id) + 1;
            }
            Shipments.Add(entity);
        }
        public void Update(Shipment entity)
        {

        }
        public void Delete(Shipment entity)
        {
            Shipments.Remove(entity);
        }
        public Task<int> SaveChangesAsync()
        {
            SaveChangesCount++;
            return Task.FromResult(1);
        }
    }
}