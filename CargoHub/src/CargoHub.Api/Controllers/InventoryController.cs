using CargoHub.Api.Dtos;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InventoriesController : ControllerBase
{
    private readonly IInventoryService _inventories;

    public InventoriesController(IInventoryService inventories)
    {
        _inventories = inventories;
    }

    [HttpGet]
    public async Task<ActionResult<List<InventoryDto>>> GetAll()
        => Ok(await _inventories.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<InventoryDto>> GetById(int id)
    {
        var inventory = await _inventories.GetByIdAsync(id);
        return inventory is null ? NotFound() : Ok(inventory);
    }

    [HttpPost]
    public async Task<ActionResult<InventoryDto>> Create(CreateInventoryDto dto)
    {
        var created = await _inventories.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<InventoryDto>> Update(int id, UpdateInventoryDto dto)
    {
        var updated = await _inventories.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var deleted = await _inventories.DeleteAsync(id);
        return deleted is false ? NotFound() : Ok(deleted);
    }
}