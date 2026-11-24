using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;

namespace CargoHub.Api.Services;

public class ShipmentService : IShipmentService
{
    private readonly IRepository<Shipment> _shipments;
    private readonly IRepository<ShipmentItem> _shipmentItems;
    private readonly IRepository<Inventory> _inventories;
    private readonly IRepository<Order> _orders;

    public ShipmentService(IRepository<Shipment> shipments, IRepository<ShipmentItem> shipmentItems, IRepository<Inventory> inventories, IRepository<Order> orders)
    {
        _shipments = shipments;
        _shipmentItems = shipmentItems;
        _inventories = inventories;
        _orders = orders;
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

    public async Task<List<ShipmentItemDto>> GetShipmentItemsInShipmentAsync(int id)
    {
        var items = await _shipmentItems.GetAllAsync();
        return items.Where(i => i.ShipmentId == id).Select(ToItemDto).ToList();
    }
    public async Task<List<int>> GetOrderIdsInShipmentAsync(int id)
    {
        var shipment = await _shipments.GetByIdAsync(id);

        if (shipment is null) return new List<int>();
        return new List<int> { shipment.OrderId };
    }

    public async Task<ShipmentDto> CreateAsync(CreateShipmentDto dto)
    {
        await DoesOrderExistAsync(dto.OrderId);

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

    public async Task<ShipmentDto?> UpdateAsync(int id, UpdateShipmentDto dto)
    {
        var shipment = await _shipments.GetByIdAsync(id);

        if (shipment is null) return null;

        await DoesOrderExistAsync(dto.OrderId);

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

    public async Task UpdateShipmentItemsInShipmentAsync(int id, List<ShipmentItemDto> shipmentItems)
    {
        var shipment = await _shipments.GetByIdAsync(id);

        if (shipment is null) throw new KeyNotFoundException($"Shipment {id} was not found.");

        var allItems = await _shipmentItems.GetAllAsync();
        var currentItems = allItems.Where(i => i.ShipmentId == id).ToList();

        var currentByItem = currentItems.GroupBy(i => i.ItemId).ToDictionary(g => g.Key, g => g.Sum(i => i.Amount));
        var newByItem = shipmentItems.GroupBy(i => i.ItemId).ToDictionary(g => g.Key, g => g.Sum(i => i.Amount));

        foreach (var itemId in currentByItem.Keys.Union(newByItem.Keys))
        {
            var delta = newByItem.GetValueOrDefault(itemId, 0) - currentByItem.GetValueOrDefault(itemId, 0);
            if (delta != 0) await ApplyOrderedDelta(itemId, delta);
        }

        foreach (var item in currentItems) _shipmentItems.Delete(item);
        foreach (var item in shipmentItems) _shipmentItems.Add(new ShipmentItem { ShipmentId = shipment.Id, ItemId = item.ItemId, Amount = item.Amount });

        await _shipmentItems.SaveChangesAsync();
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        var shipment = await _shipments.GetByIdAsync(id);

        if (shipment is null) return null;

        var items = await _shipmentItems.GetAllAsync();
        foreach (var item in items.Where(i => i.ShipmentId == id).ToList())
        {
            _shipmentItems.Delete(item);
        }
        await _shipmentItems.SaveChangesAsync();

        _shipments.Delete(shipment);
        await _shipments.SaveChangesAsync();

        return true;
    }

    private async Task DoesOrderExistAsync(int orderId)
    {
        var order = await _orders.GetByIdAsync(orderId);
        if (order is null) throw new ArgumentException($"Order {orderId} does not exist.", nameof(orderId));
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
            PaymentType = shipment.PaymentType,
            CreatedAt = shipment.CreatedAt,
            UpdatedAt = shipment.UpdatedAt
        };
    }

    private static ShipmentItemDto ToItemDto(ShipmentItem shipmentItem)
    {
        return new ShipmentItemDto
        {
            Id = shipmentItem.Id,
            ShipmentId = shipmentItem.ShipmentId,
            ItemId = shipmentItem.ItemId,
            Amount = shipmentItem.Amount
        };
    }

    private async Task ApplyOrderedDelta(int itemId, int delta)
    {
        var inventories = await _inventories.GetAllAsync();
        var inventoriesForItem = inventories.Where(i => i.ItemId == itemId).ToList();

        if (!inventoriesForItem.Any()) return;

        var target = inventoriesForItem.MaxBy(i => i.QuantityOnHand)!;
        target.QuantityOrdered = Math.Max(0, target.QuantityOrdered + delta);

        await _inventories.SaveChangesAsync();
    }
}