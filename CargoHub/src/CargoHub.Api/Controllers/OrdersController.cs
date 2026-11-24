using CargoHub.Api.Dtos;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Http.HttpResults;
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
        return orderItems is null ? NotFound() : Ok(orderItems);
    }

    [HttpGet("client/{clientId:int}")]
    [HttpGet("~/api/clients/{clientId:int}/orders")]
    public async Task<ActionResult<List<OrderDto>>> GetOrdersByClientId(int clientId) => Ok(await _orders.GetOrdersByClientIdAsync(clientId));

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(CreateOrderDto dto)
    {
        try
        {
            var created = await _orders.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<OrderDto?>> Update(int id, UpdateOrderDto dto)
    {
        try
        {
            var updated = await _orders.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
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
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool?>> Delete(int id)
    {
        try
        {
            var deleted = await _orders.DeleteAsync(id);
            return deleted is null ? NotFound() : Ok(deleted);
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { error = ex.Message });
        }
    }
}