using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;
using CargoHub.Api.Dtos;

namespace CargoHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TransfersController : ControllerBase
{
    private readonly ITransferService _transfers;
    public TransfersController(ITransferService transfers)
    {
        _transfers = transfers;
    }

    [HttpGet]
    public async Task<ActionResult<List<TransferDto>>> GetAll() => Ok(await _transfers.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransferDto>> GetById(int id)
    {
        var transfer = await _transfers.GetByIdAsync(id);
        return transfer is null ? NotFound() : Ok(transfer);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<List<TransferItemDto>>> GetItemsInTransfer(int id)
    {
        var transfers = await _transfers.GetItemsInTransferAsync(id);
        return Ok(transfers);
    }

    [HttpPost]
    public async Task<ActionResult<TransferDto>> Create(CreateTransferDto dto)
    {
        var created = await _transfers.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<TransferDto?>> Update(int id, UpdateTransferDto dto)
    {
        var updated = await _transfers.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool?>> Delete(int id)
    {
        var deleted = await _transfers.DeleteAsync(id);
        return deleted is null ? NotFound() : Ok(deleted);
    }
}