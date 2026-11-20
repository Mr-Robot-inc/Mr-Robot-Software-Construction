using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;

namespace CargoHub.Api.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _orders;
    private readonly IRepository<OrderItem> _orderItems;

    public OrderService(IRepository<Order> orders, IRepository<OrderItem> orderItems)
    {
        _orders = orders;
        _orderItems = orderItems;
    }

    public async Task<List<OrderDto>> GetAllAsync()
    {
        var orders = await _orders.GetAllAsync();

        var result = new List<OrderDto>();

        foreach (var order in orders)
        {
            result.Add(await ToDtoAsync(order));
        }

        return result;
    }

    public async Task<OrderDto?> GetByIdAsync(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        return order is null ? null : await ToDtoAsync(order);
    }

    public async Task<List<OrderItemDto>> GetOrderItemsInOrderAsync(int orderId)
    {
        var items = await _orderItems.GetAllAsync();

        return items.Where(i => i.OrderId == orderId).Select(ToItemDto).ToList();
    }

    public async Task<List<OrderDto>> GetOrdersByClientIdAsync(int clientId)
    {
        var orders = await _orders.GetAllAsync();
        var filteredOrders = orders.Where(o => o.ClientId == clientId || o.ShipToClientId == clientId || o.BillToClientId == clientId);

        var result = new List<OrderDto>();
        foreach (var order in filteredOrders)
        {
            result.Add(await ToDtoAsync(order));
        }

        return result;
    }

    public async Task<OrderDto> CreateAsync(CreateOrderDto dto)
    {

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
            BillToClientId = dto.BillToClientId,
        };

        _orders.Add(order);
        await _orders.SaveChangesAsync();

        return await ToDtoAsync(order);
    }
    public async Task<OrderDto?> UpdateAsync(int id, UpdateOrderDto dto)
    {
        var order = await _orders.GetByIdAsync(id);

        if (order is null) return null;
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

        return await ToDtoAsync(order);
    }
    public async Task UpdateOrderItemsInOrderAsync(int id, List<OrderItemDto> orderItems)
    {
        var order = await _orders.GetByIdAsync(id);

        if (order is null) throw new KeyNotFoundException($"Order {id} was not found.");

        var items = await _orderItems.GetAllAsync();
        var itemsToRemove = items.Where(i => i.OrderId == id).ToList();

        foreach (var item in itemsToRemove)
        {
            _orderItems.Delete(item);
        }

        foreach (var item in orderItems)
        {
            var newItem = new OrderItem
            {
                OrderId = order.Id,
                ItemId = item.ItemId,
                Amount = item.Amount
            };

            _orderItems.Add(newItem); 
        }
        await _orderItems.SaveChangesAsync();
    }
    public async Task<bool?> DeleteAsync(int id)
    {
        var order = await _orders.GetByIdAsync(id);

        if (order is null) return null;

        var items = await _orderItems.GetAllAsync();

        var itemsToRemove = items.Where(i => i.OrderId == id).ToList();
        foreach (var item in itemsToRemove)
        {
            _orderItems.Delete(item);
        }
        await _orderItems.SaveChangesAsync();

        _orders.Delete(order);
        await _orders.SaveChangesAsync();

        return true;
    }

    private async Task<OrderDto> ToDtoAsync(Order order)
    {
        var items = await GetOrderItemsInOrderAsync((order.Id));
        return new OrderDto
        {
            Id = order.Id,
            ClientId = order.ClientId,
            OrderDate = order.OrderDate,
            RequestDate = order.RequestDate,
            Reference = order.Reference!,
            CustomerPoNumber = order.CustomerPoNumber!,
            OrderStatus = order.OrderStatus!,
            ShippingNotes = order.ShippingNotes,
            WarehouseId = order.WarehouseId,
            ShipToClientId = order.ShipToClientId,
            BillToClientId = order.BillToClientId,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        };
    }

    private static OrderItemDto ToItemDto(OrderItem orderItem)
    {
        return new OrderItemDto
        {
            Id = orderItem.Id,
            OrderId = orderItem.OrderId,
            ItemId = orderItem.ItemId,
            Amount = orderItem.Amount
        };
    }
}