using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;

namespace CargoHub.Api.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orders;
    private readonly IRepository<OrderItem> _orderItems;
    private readonly IRepository<Client> _clients;
    private readonly IRepository<Warehouse> _warehouses;
    private readonly IRepository<Item> _items;
    private readonly IRepository<Inventory> _inventories;
    private readonly IRepository<Shipment> _shipments;

    public OrderService(
        IRepository<Order> orders,
        IRepository<OrderItem> orderItems,
        IRepository<Client> clients,
        IRepository<Warehouse> warehouses,
        IRepository<Item> items,
        IRepository<Inventory> inventories,
        IRepository<Shipment> shipments)
    {
        _orders = orders;
        _orderItems = orderItems;
        _clients = clients;
        _warehouses = warehouses;
        _items = items;
        _inventories = inventories;
        _shipments = shipments;
    }

    public async Task<List<OrderDto>> GetAllAsync()
    {
        var orders = await _orders.GetAllAsync();
        var items = await _orderItems.GetAllAsync();
        return ToDtos(orders, items);
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order is null) return null;

        var items = await _orderItems.FindAsync(i => i.OrderId == id);
        return ToDto(order, items.Select(ToItemDto).ToList());
    }

    public async Task<List<OrderItemDto>> GetOrderItemsInOrderAsync(int orderId)
    {
        var items = await _orderItems.FindAsync(i => i.OrderId == orderId);
        return items.Select(ToItemDto).ToList();
    }

    public async Task<List<OrderDto>> GetOrdersByClientIdAsync(int clientId)
    {
        var orders = await _orders.FindAsync(o =>
            o.ClientId == clientId ||
            o.ShipToClientId == clientId ||
            o.BillToClientId == clientId);
        if (orders.Count == 0) return new List<OrderDto>();

        var orderIds = orders.Select(o => o.Id).ToList();
        var orderItems = await _orderItems.FindAsync(i => orderIds.Contains(i.OrderId));
        return ToDtos(orders, orderItems);
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
    {
        await DoIdsExistAsync(dto.ClientId, dto.ShipToClientId, dto.BillToClientId, dto.WarehouseId);
        await DoItemsExistAsync(dto.Items);

        var order = new Order
        {
            ClientId = dto.ClientId,
            OrderDate = dto.OrderDate,
            RequestDate = dto.RequestDate,
            Reference = dto.Reference,
            CustomerPoNumber = dto.CustomerPoNumber,
            OrderStatus = dto.OrderStatus,
            ShippingNotes = dto.ShippingNotes,
            WarehouseId = dto.WarehouseId,
            ShipToClientId = dto.ShipToClientId,
            BillToClientId = dto.BillToClientId
        };

        _orders.Add(order);
        await _orders.SaveChangesAsync();

        if (dto.Items.Count > 0)
        {
            AddItems(order.Id, dto.Items);
            await _orderItems.SaveChangesAsync();
        }

        var savedItems = dto.Items.Count > 0
            ? (await _orderItems.FindAsync(i => i.OrderId == order.Id)).Select(ToItemDto).ToList()
            : new List<OrderItemDto>();
        return ToDto(order, savedItems);
    }

    public async Task<OrderDto?> UpdateAsync(int id, UpdateOrderDto dto)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order is null) return null;

        await DoIdsExistAsync(dto.ClientId, dto.ShipToClientId, dto.BillToClientId, dto.WarehouseId);

        if (dto.Items is not null) await ReplaceItemsAsync(id, dto.Items);

        order.ClientId = dto.ClientId;
        order.OrderDate = dto.OrderDate;
        order.RequestDate = dto.RequestDate;
        order.Reference = dto.Reference;
        order.CustomerPoNumber = dto.CustomerPoNumber;
        order.OrderStatus = dto.OrderStatus;
        order.ShippingNotes = dto.ShippingNotes;
        order.WarehouseId = dto.WarehouseId;
        order.ShipToClientId = dto.ShipToClientId;
        order.BillToClientId = dto.BillToClientId;

        await _orders.SaveChangesAsync();

        var items = await _orderItems.FindAsync(i => i.OrderId == id);
        return ToDto(order, items.Select(ToItemDto).ToList());
    }

    public async Task UpdateOrderItemsInOrderAsync(int id, List<OrderItemDto> orderItems)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order is null) throw new KeyNotFoundException($"Order {id} was not found.");

        await ReplaceItemsAsync(id, orderItems);
        await _orderItems.SaveChangesAsync();
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        if (order is null) return null;

        if ((await _shipments.FindAsync(s => s.OrderId == id)).Count > 0) throw new InvalidOperationException($"Order {id} still has shipments and cannot be deleted.");

        foreach (var item in await _orderItems.FindAsync(i => i.OrderId == id))
        {
            _orderItems.Delete(item);
        }

        _orders.Delete(order);
        await _orders.SaveChangesAsync();

        return true;
    }

    private async Task ReplaceItemsAsync(int orderId, List<OrderItemDto> newItems)
    {
        await DoItemsExistAsync(newItems);

        var currentItems = await _orderItems.FindAsync(i => i.OrderId == orderId);

        var currentByItem = currentItems.GroupBy(i => i.ItemId).ToDictionary(g => g.Key, g => g.Sum(i => i.Amount));
        var newByItem = newItems.GroupBy(i => i.ItemId).ToDictionary(g => g.Key, g => g.Sum(i => i.Amount));

        await ApplyAllocationDeltasAsync(currentByItem, newByItem);

        foreach (var item in currentItems) _orderItems.Delete(item);
        AddItems(orderId, newItems);
    }

    private async Task ApplyAllocationDeltasAsync(Dictionary<int, int> currentByItem, Dictionary<int, int> newByItem)
    {
        var deltas = currentByItem.Keys.Union(newByItem.Keys)
            .Select(itemId => (ItemId: itemId, Delta: newByItem.GetValueOrDefault(itemId) - currentByItem.GetValueOrDefault(itemId)))
            .Where(x => x.Delta != 0)
            .ToList();
        if (deltas.Count == 0) return;

        var itemIds = deltas.Select(d => d.ItemId).ToList();
        var inventories = await _inventories.FindAsync(i => itemIds.Contains(i.ItemId));
        var target = inventories
            .GroupBy(i => i.ItemId)
            .ToDictionary(g => g.Key, g => g.MaxBy(i => i.QuantityOnHand)!);

        foreach (var (itemId, delta) in deltas)
        {
            if (target.TryGetValue(itemId, out var inventory))
                inventory.QuantityAllocated = Math.Max(0, inventory.QuantityAllocated + delta);
        }
    }

    private void AddItems(int orderId, List<OrderItemDto> items)
    {
        foreach (var item in items)
        {
            _orderItems.Add(new OrderItem
            {
                OrderId = orderId,
                ItemId = item.ItemId,
                Amount = item.Amount,
                UnitPrice = item.UnitPrice
            });
        }
    }

    private async Task DoIdsExistAsync(int clientId, int shipToClientId, int billToClientId, int warehouseId)
    {
        var clientIds = new[] { clientId, shipToClientId, billToClientId }.Distinct().ToList();
        var found = (await _clients.FindAsync(c => clientIds.Contains(c.Id))).Select(c => c.Id).ToHashSet();
        var missingClient = clientIds.Where(id => !found.Contains(id)).Cast<int?>().FirstOrDefault();
        if (missingClient is not null)
            throw new ArgumentException($"Client {missingClient} does not exist.");

        if (await _warehouses.GetByIdAsync(warehouseId) is null)
            throw new ArgumentException($"Warehouse {warehouseId} does not exist.");
    }

    private async Task DoItemsExistAsync(List<OrderItemDto> items)
    {
        var itemIds = items.Select(i => i.ItemId).Distinct().ToList();
        if (itemIds.Count == 0) return;

        var found = (await _items.FindAsync(i => itemIds.Contains(i.Id))).Select(i => i.Id).ToHashSet();
        var missing = itemIds.Where(id => !found.Contains(id)).Cast<int?>().FirstOrDefault();
        if (missing is not null)
            throw new ArgumentException($"Item {missing} does not exist.");
    }

    private static List<OrderDto> ToDtos(List<Order> orders, List<OrderItem> allItems)
    {
        var itemsByOrder = allItems
            .GroupBy(i => i.OrderId)
            .ToDictionary(g => g.Key, g => g.Select(ToItemDto).ToList());

        return orders.Select(o => ToDto(o, itemsByOrder.GetValueOrDefault(o.Id) ?? new List<OrderItemDto>())).ToList();
    }

    private static OrderDto ToDto(Order order, List<OrderItemDto> items)
    {
        return new OrderDto
        {
            Id = order.Id,
            ClientId = order.ClientId,
            OrderDate = order.OrderDate,
            RequestDate = order.RequestDate,
            Reference = order.Reference,
            CustomerPoNumber = order.CustomerPoNumber,
            OrderStatus = order.OrderStatus,
            ShippingNotes = order.ShippingNotes,
            WarehouseId = order.WarehouseId,
            ShipToClientId = order.ShipToClientId,
            BillToClientId = order.BillToClientId,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt,
            Items = items
        };
    }

    private static OrderItemDto ToItemDto(OrderItem orderItem)
    {
        return new OrderItemDto
        {
            Id = orderItem.Id,
            OrderId = orderItem.OrderId,
            ItemId = orderItem.ItemId,
            Amount = orderItem.Amount,
        };
    }
}