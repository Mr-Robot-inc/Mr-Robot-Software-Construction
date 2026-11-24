using CargoHub.Api.Dtos;

namespace CargoHub.Api.Services;

public interface IOrderService
{
    Task<List<OrderDto>> GetAllAsync();
    Task<OrderDto?> GetByIdAsync(int id);
    Task<List<OrderItemDto>> GetOrderItemsInOrderAsync(int orderId);
    Task<List<OrderDto>> GetOrdersByClientIdAsync(int clientId);
    Task<OrderDto> CreateAsync(CreateOrderDto dto);
    Task<OrderDto?> UpdateAsync(int id, UpdateOrderDto dto);
    Task UpdateOrderItemsInOrderAsync(int id, List<OrderItemDto> orderItems);
    Task<bool?> DeleteAsync(int id);
}