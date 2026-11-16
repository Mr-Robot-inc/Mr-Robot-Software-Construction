using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemLinesController : ControllerBase
{
    private readonly IItemLineService _itemLines;
    private readonly IItemService _items;

    public ItemLinesController(IItemLineService itemLines, IItemService items)
    {
        _itemLines = itemLines;
        _items = items;
    }

    [HttpGet]
    public async Task<ActionResult<List<ItemLineDto>>> GetAll()
      => Ok(await _itemLines.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ItemLineDto>> GetById(int id)
    {
        var item = await _itemLines.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("{id:int}/items")]
    public async Task<ActionResult<List<ItemDto>>> GetItemsByLine(int id)
    {
        var itemLine = await _itemLines.GetByIdAsync(id);
        if (itemLine is null)
            return NotFound();

        var items = await _items.GetItemsByItemLineIdAsync(id);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ItemLineDto>> Create(CreateItemLineDto dto)
    {
        var created = await _itemLines.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ItemLineDto>> Update(int id, UpdateItemLineDto dto)
    {
        var updated = await _itemLines.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var deleted = await _itemLines.DeleteAsync(id);
        return deleted is false ? NotFound() : Ok(deleted);
    }
}