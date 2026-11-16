using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemTypesController : ControllerBase
{
    private readonly IItemTypeService _itemTypes;
    private readonly IItemService _items;

    public ItemTypesController(IItemTypeService itemTypes, IItemService items)
    {
        _itemTypes = itemTypes;
        _items = items;
    }

    [HttpGet]
    public async Task<ActionResult<List<ItemTypeDto>>> GetAll()
      => Ok(await _itemTypes.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ItemTypeDto>> GetById(int id)
    {
        var item = await _itemTypes.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("{id:int}/items")]
    public async Task<ActionResult<List<ItemDto>>> GetItemsByType(int id)
    {
        var itemType = await _itemTypes.GetByIdAsync(id);
        if (itemType is null)
            return NotFound();

        var items = await _items.GetItemsByItemTypeIdAsync(id);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ItemTypeDto>> Create(CreateItemTypeDto dto)
    {
        var created = await _itemTypes.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ItemTypeDto>> Update(int id, UpdateItemTypeDto dto)
    {
        var updated = await _itemTypes.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var deleted = await _itemTypes.DeleteAsync(id);
        return deleted is false ? NotFound() : Ok(deleted);
    }
}
