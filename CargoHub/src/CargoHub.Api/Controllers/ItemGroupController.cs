using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace CargoHub.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ItemGroupsController : ControllerBase
{
    private readonly IItemGroupService _itemGroups;
    private readonly IItemService _items;

    public ItemGroupsController(IItemGroupService itemGroups, IItemService items)
    {
        _itemGroups = itemGroups;
        _items = items;
    }

    [HttpGet]
    public async Task<ActionResult<List<ItemGroupDto>>> GetAll()
      => Ok(await _itemGroups.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<ItemGroupDto>> GetById(int id)
    {
        var item = await _itemGroups.GetByIdAsync(id);
        return item is null ? NotFound() : Ok(item);
    }

    [HttpGet("{id:int}/items")]
    public async Task<ActionResult<List<ItemDto>>> GetItemsByGroup(int id)
    {
        var itemGroup = await _itemGroups.GetByIdAsync(id);
        if (itemGroup is null)
            return NotFound();

        var items = await _items.GetItemsByItemGroupIdAsync(id);
        return Ok(items);
    }

    [HttpPost]
    public async Task<ActionResult<ItemGroupDto>> Create(CreateItemGroupDto dto)
    {
        var created = await _itemGroups.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ItemGroupDto>> Update(int id, UpdateItemGroupDto dto)
    {
        var updated = await _itemGroups.UpdateAsync(id, dto);
        return updated is null ? NotFound() : Ok(updated);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<bool>> Delete(int id)
    {
        var deleted = await _itemGroups.DeleteAsync(id);
        return deleted is false ? NotFound() : Ok(deleted);
    }
}