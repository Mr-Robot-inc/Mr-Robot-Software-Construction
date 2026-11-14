using CargoHub.Api.Dtos;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SuppliersController : ControllerBase
{
    private readonly ISupplierService _supplierService;
    public SuppliersController(ISupplierService supplierService)
    {
        _supplierService = supplierService;
    }
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SupplierDto>>> GetAll()
    {
        var suppliers = await _supplierService.GetAll();
        return Ok(suppliers);
    }
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SupplierDto>> GetById(int id)
    {
        var supplier = await _supplierService.GetSupplierByIdAsync(id);
        return supplier is null ? NotFound() : Ok(supplier);
    }
    [HttpPost]
    public async Task<ActionResult<SupplierDto>> Create(CreateSupplierDto dto)
    {
        var createdSupplier = await _supplierService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdSupplier.Id }, createdSupplier);
    }
    [HttpPut("{id:int}")]
    public async Task<ActionResult<SupplierDto>> Update(int id, UpdateSupplierDto dto)
    {
        var updatedSupplier = await _supplierService.UpdateAsync(id, dto);
        return updatedSupplier is null ? NotFound() : Ok(updatedSupplier);
    }
    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var deleted = await _supplierService.DeleteAsync(id);
        return deleted is false ? NotFound() : Ok(deleted);
    }
}