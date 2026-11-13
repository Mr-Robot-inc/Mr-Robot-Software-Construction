using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPost]
    public async Task<ActionResult<ShipmentDto>> Create(CreateShipmentDto dto)
    {
        var created = await _shipments.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ShipmentDto>> Update(int id, UpdateShipmentDto dto)
    {
        var updated = await _shipments.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ShipmentDto>> Delete(int id)
    {
        var deleted = await _shipments.DeleteAsync(id);
        return deleted is null ? NotFound() : Ok(deleted);
    }
}