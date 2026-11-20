using CargoHub.Api.Dtos;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrdersController : ControllerBase
{
    private readonly IOrderService _orders;
    public OrdersController(IOrderService orders)
    {
        _orders = orders;
    }

    [HttpGet]
    public async Task<ActionResult<List<OrderDto>>> GetAll() => Ok(await _orders.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<OrderDto>> GetById(int id)
    {
        var order = await _orders.GetByIdAsync(id);
        return order is null ? NotFound() : Ok(order);
    }

    [HttpGet("{id:int}/items")]
    public async Task<ActionResult<List<OrderItemDto>>> GetOrderItemsInOrder(int id)
    {
        var orderItems = await _orders.GetOrderItemsInOrderAsync(id);
        return Ok(orderItems);
    }

    [HttpGet("client/{clientId:int}")]
    public async Task<ActionResult<List<OrderDto>>> GetOrdersByClientId(int clientId)
    {
        var orders = await _orders.GetOrdersByClientIdAsync(clientId);
        return orders.Any() ? Ok(orders) : NotFound();
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto)
    {
        var created = await _orders.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<OrderDto?>> Update(int id, UpdateOrderDto dto)
    {
        var updated = await _orders.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpPut("{id:int}/items")]
    public async Task<ActionResult> UpdateOrderItemsInOrder(int id, List<OrderItemDto> orderItems)
    {
        try
        {
            await _orders.UpdateOrderItemsInOrderAsync(id, orderItems);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool?>> Delete(int id)
    {
        var deleted = await _orders.DeleteAsync(id);
        return deleted is null ? NotFound() : Ok(deleted);
    }
}