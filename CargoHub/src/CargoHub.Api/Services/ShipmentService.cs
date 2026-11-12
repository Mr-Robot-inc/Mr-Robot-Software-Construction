using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;

namespace CargoHub.Api.Services;

public class ShipmentService : IShipmentService
{
    private readonly IRepository<Shipment> _shipments;

    public ShipmentService(IRepository<Shipment> shipments)
    {
        _shipments = shipments;
    }

    public async Task<List<ShipmentDto>> GetAllAsync()
    {
        var shipments = await _shipments.GetAllAsync();
        return shipments.Select(ToDto).ToList();
    }

    public async Task<ShipmentDto?> GetByIdAsync(int id)
    {
        var shipment = await _shipments.GetByIdAsync(id);
        return shipment is null ? null : ToDto(shipment);
    }

    public async Task<List<int>> GetOrderIdsInShipmentAsync(int id)
    {
        var shipment = await _shipments.GetByIdAsync(id);

        if (shipment is null) return new List<int>();
        return new List<int> { shipment.OrderId };
    }

    public async Task<ShipmentDto> CreateAsync(CreateShipmentDto dto)
    {
        var shipment = new Shipment
        {
            Reference = dto.Reference,
            OrderId = dto.OrderId,
            ShipmentDate = dto.ShipmentDate,
            ShipmentType = dto.ShipmentType,
            ShipmentStatus = dto.ShipmentStatus,
            CarrierName = dto.CarrierName,
            ShippingMethod = dto.ShippingMethod,
            PaymentType = dto.PaymentType
        };
        _shipments.Add(shipment);

        await _shipments.SaveChangesAsync();
        return ToDto(shipment);
    }

    public async Task<ShipmentDto> UpdateAsync(int id, UpdateShipmentDto dto)
    {
        var shipment = await _shipments.GetByIdAsync(id);

        if (shipment is null) return null;

        shipment.Reference = dto.Reference;
        shipment.OrderId = dto.OrderId;
        shipment.ShipmentDate = dto.ShipmentDate;
        shipment.ShipmentType = dto.ShipmentType;
        shipment.ShipmentStatus = dto.ShipmentStatus;
        shipment.CarrierName = dto.CarrierName;
        shipment.ShippingMethod = dto.ShippingMethod;
        shipment.PaymentType = dto.PaymentType;

        await _shipments.SaveChangesAsync();
        return ToDto(shipment);
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        var shipment = await _shipments.GetByIdAsync(id);

        if (shipment is null) return null;
        _shipments.Delete(shipment);

        await _shipments.SaveChangesAsync();
        return true;
    }

    private static ShipmentDto ToDto(Shipment shipment)
    {
        return new ShipmentDto
        {
            Id = shipment.Id,
            Reference = shipment.Reference,
            OrderId = shipment.OrderId,
            ShipmentDate = shipment.ShipmentDate,
            ShipmentType = shipment.ShipmentType,
            ShipmentStatus = shipment.ShipmentStatus,
            CarrierName = shipment.CarrierName,
            ShippingMethod = shipment.ShippingMethod,
            PaymentType = shipment.PaymentType
        };
    }
}