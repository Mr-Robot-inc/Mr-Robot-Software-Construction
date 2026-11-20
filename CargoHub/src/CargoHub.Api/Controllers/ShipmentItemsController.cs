using CargoHub.Api.Dtos;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;
namespace CargoHub.Api.Controllers;
[ApiController]
[Route("api/[controller]")]
public class ShipmentItemsController : ControllerBase 
{
    private readonly IShipmentItemService _shipmentitems;

    public ShipmentItemsController(IShipmentItemService clients)
    {
        _shipmentitems = clients;
    }

    [HttpGet]
    public async Task<ActionResult<List<ShipmentItemDto>>> GetAll()
        => Ok(await _shipmentitems.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ShipmentItemDto>> GetById(int id)
    {
        var client = await _shipmentitems.GetByIdAsync(id);
        return client is null ? NotFound() : Ok(client);
    }

    [HttpPost]
    public async Task<ActionResult<ShipmentItemDto>> Create(CreateShipmentItemDto dto)
    {
        var created = await _shipmentitems.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ShipmentItemDto>> Update(int id, UpdateShipmentItemDto dto)
    {
        var updated = await _shipmentitems.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var deleted = await _shipmentitems.DeleteAsync(id);
        return deleted is false ? NotFound() : NoContent();
    }

}

