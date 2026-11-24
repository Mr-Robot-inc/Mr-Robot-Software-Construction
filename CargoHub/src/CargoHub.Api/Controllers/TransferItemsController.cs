using CargoHub.Api.Dtos;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransferItemsController : ControllerBase
{
    private readonly ITransferItemService _transferItems;

    public TransferItemsController(ITransferItemService transferItems)
    {
        _transferItems = transferItems;
    }

    [HttpGet]
    public async Task<ActionResult<List<TransferItemDto>>> GetAll() => Ok(await _transferItems.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransferItemDto>> GetById(int id)
    {
        var transferItem = await _transferItems.GetByIdAsync(id);
        return transferItem is null ? NotFound() : Ok(transferItem);
    }

    [HttpGet("transfer/{transferId:int}")]
    public async Task<ActionResult<List<TransferItemDto>>> GetItemsForTransfer(int transferId) => Ok(await _transferItems.GetItemsForTransferAsync(transferId));

    [HttpPost]
    public async Task<ActionResult<TransferItemDto>> Create(CreateTransferItemDto dto)
    {
        try
        {
            var created = await _transferItems.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TransferItemDto>> Update(int id, UpdateTransferItemDto dto)
    {
        try
        {
            var updated = await _transferItems.UpdateAsync(id, dto);
            return updated is null ? NotFound() : Ok(updated);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    [HttpPut("transfer/{transferId:int}")]
    public async Task<ActionResult> SetItemsForTransfer(int transferId, List<TransferItemDto> transferItems)
    {
        try
        {
            await _transferItems.SetItemsForTransferAsync(transferId, transferItems);
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
        var deleted = await _transferItems.DeleteAsync(id);
        return deleted is null ? NotFound() : Ok(deleted);
    }

    [HttpDelete("transfer/{transferId:int}")]
    public async Task<ActionResult<bool?>> RemoveItemsForTransfer(int transferId)
    {
        var deleted = await _transferItems.RemoveItemsForTransferAsync(transferId);
        return deleted is null ? NotFound() : Ok(deleted);
    }
}
