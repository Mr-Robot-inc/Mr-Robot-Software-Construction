using CargoHub.Api.Dtos;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransferItemController : ControllerBase
{
    private readonly ITransferItemService _transferItemService;

    public TransferItemController(ITransferItemService transferItemService)
    {
        _transferItemService = transferItemService;
    }

    [HttpGet]
    public async Task<ActionResult<List<TransferItemDto>>> GetAll()
        => Ok(await _transferItemService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransferItemDto?>> GetById(int id)
    {
        var transferItem = await _transferItemService.GetByIdAsync(id);
        return transferItem is null ? NotFound() : Ok(transferItem);
    }

    // might add later get by transfer id and item id

    [HttpPost]
    public async Task<ActionResult<TransferItemDto>> Create(CreateTransferItemDto dto)
    {
        var created = await _transferItemService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TransferItemDto?>> Update(int id, UpdateTransferItemDto dto)
    {
        var updated = await _transferItemService.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var deleted = await _transferItemService.DeleteAsync(id);
        return deleted ? Ok(true) : NotFound();
    }
}
