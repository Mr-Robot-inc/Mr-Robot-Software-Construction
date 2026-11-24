using CargoHub.Api.Dtos;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CargoHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ShipmentsController : ControllerBase
{
    private readonly IShipmentService _shipments;

    public ShipmentsController(IShipmentService shipments)
    {
        _shipments = shipments;
    }

    [HttpGet]
    public async Task<ActionResult<List<ShipmentDto>>> GetAll() => Ok(await _shipments.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ShipmentDto>> GetById(int id)
    {
        var shipment = await _shipments.GetByIdAsync(id);
        return shipment is null ? NotFound() : Ok(shipment);
    }

    [HttpGet("{id:int}/orders")]
    public async Task<ActionResult<List<int>>> GetOrderIds(int id)
    {
        var orderIds = await _shipments.GetOrderIdsInShipmentAsync(id);

        if (orderIds.Count == 0) return NotFound();
        return Ok(orderIds);
    }

    [HttpGet("{id:int}/items")]
    public async Task<ActionResult<List<ShipmentItemDto>>> GetShipmentItemsInShipment(int id)
    {
        if (await _shipments.GetByIdAsync(id) is null) return NotFound();
        return Ok(await _shipments.GetShipmentItemsInShipmentAsync(id));
    }

    [HttpPost]
    public async Task<ActionResult<ShipmentDto>> Create(CreateShipmentDto dto)
    {
        try
        {
            var created = await _shipments.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ShipmentDto>> Update(int id, UpdateShipmentDto dto)
    {
        try
        {
            var updated = await _shipments.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}/items")]
    public async Task<ActionResult> UpdateShipmentItemsInShipment(int id, List<ShipmentItemDto> shipmentItems)
    {
        try
        {
            await _shipments.UpdateShipmentItemsInShipmentAsync(id, shipmentItems);
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
        var deleted = await _shipments.DeleteAsync(id);
        return deleted is null ? NotFound() : Ok(deleted);
    }
}