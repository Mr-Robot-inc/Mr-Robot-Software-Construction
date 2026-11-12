using CargoHub.Api.Dtos;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouses;
    public WarehousesController(IWarehouseService warehouses)
    {
        _warehouses = warehouses;
    }
    [HttpGet]
    public async Task<ActionResult<List<WarehouseDto>>> GetAll()
        => Ok(await _warehouses.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<WarehouseDto>> GetById(int id)
    {
        var warehouse = await _warehouses.GetByIdAsync(id);
        return warehouse is null ? NotFound() : Ok(warehouse);
    }
    [HttpPost]
    public async Task<ActionResult<WarehouseDto>> Create(CreateWarehouseDto dto)
    {
        var created = await _warehouses.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult<WarehouseDto>> Update(int id, UpdatedWarehouseDto dto)
    {
        var updated = await _warehouses.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var deleted = await _warehouses.DeleteAsync(id);
        return deleted is false ? NotFound() : Ok(deleted);
    }
}