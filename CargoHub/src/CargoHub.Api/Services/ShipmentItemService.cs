
using CargoHub.Api.Models;
using CargoHub.Api.Dtos;
using CargoHub.Api.Repositories;

namespace CargoHub.Api.Services;

public class ShipmentItemService : IShipmentItemService
{
    private readonly IRepository<ShipmentItem> _repository;

    public ShipmentItemService(IRepository<ShipmentItem> repository)
    {
        _repository = repository;
    }

    public async Task<List<ShipmentItemDto>> GetAllAsync()
    {
        var shipmentItems = await _repository.GetAllWithDetailsAsync(
            "Shipment",
            "Item"
        );

        return shipmentItems.Select(ToDto).ToList();
    }

    public async Task<ShipmentItemDto?> GetByIdAsync(int id)
    {
        var shipmentItem = await _repository.GetByIdWithDetailsAsync(
            id,
            "Shipment",
            "Item"
        );

        return shipmentItem is null ? null : ToDto(shipmentItem);
    }

    public async Task<ShipmentItemDto> CreateAsync(CreateShipmentItemDto dto)
    {
        var newShipmentItem = new ShipmentItem
        {
            ShipmentId = dto.ShipmentId,
            ItemId = dto.ItemId,
            Amount = dto.Amount
        };

        _repository.Add(newShipmentItem);

        await _repository.SaveChangesAsync();

        var created = await _repository.GetByIdWithDetailsAsync(
            newShipmentItem.Id,
            "Shipment",
            "Item"
        );

        if (created is null)
            throw new InvalidOperationException(
                "Could not find the created shipment item."
            );

        return ToDto(created);
    }

    public async Task<ShipmentItemDto?> UpdateAsync(
        int id,
        UpdateShipmentItemDto dto)
    {
        var shipmentItem = await _repository.GetByIdAsync(id);

        if (shipmentItem is null)
            return null;

        shipmentItem.ShipmentId = dto.ShipmentId;
        shipmentItem.ItemId = dto.ItemId;
        shipmentItem.Amount = dto.Amount;

        await _repository.SaveChangesAsync();

        var updated = await _repository.GetByIdWithDetailsAsync(
            id,
            "Shipment",
            "Item"
        );

        return updated is null ? null : ToDto(updated);
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        var shipmentItem = await _repository.GetByIdAsync(id);

        if (shipmentItem is null)
            return false;

        _repository.Delete(shipmentItem);

        await _repository.SaveChangesAsync();

        return true;
    }

    private static ShipmentItemDto ToDto(ShipmentItem shipmentItem)
    {
        if (shipmentItem.Shipment is null || shipmentItem.Item is null)
        {
            throw new InvalidOperationException(
                "ShipmentItem must have both Shipment and Item loaded."
            );
        }

        return new ShipmentItemDto
        {
            Id = shipmentItem.Id,

            Shipment = new ShipmentShipmentItemDto
            {
                Id = shipmentItem.Shipment.Id,
                Reference = shipmentItem.Shipment.Reference,
                OrderId = shipmentItem.Shipment.OrderId,
                ShipmentType = shipmentItem.Shipment.ShipmentType,
                ShipmentStatus = shipmentItem.Shipment.ShipmentStatus,
                CarrierName = shipmentItem.Shipment.CarrierName,
                ShippingMethod = shipmentItem.Shipment.ShippingMethod,
                PaymentType = shipmentItem.Shipment.PaymentType
            },

            Item = new ItemShimpentDto
            {
                Id = shipmentItem.Item.Id,
                Code = shipmentItem.Item.Code,
                Description = shipmentItem.Item.Description,
                BarCode = shipmentItem.Item.BarCode,
                ModeNumber = shipmentItem.Item.ModeNumber,
                CommodityCode = shipmentItem.Item.CommodityCode,
                UnitWeight = shipmentItem.Item.UnitWeight,
                CaseSize = shipmentItem.Item.CaseSize,
                PackagingType = shipmentItem.Item.PackagingType,
                OrderMultiple = shipmentItem.Item.OrderMultiple
            },

            Amount = shipmentItem.Amount
        };
    }
}
