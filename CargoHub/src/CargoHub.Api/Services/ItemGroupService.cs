using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;

namespace CargoHub.Api.Services;

public class ItemGroupService : IItemGroupService
{
    private readonly IRepository<ItemGroup> _itemGroups;

    public ItemGroupService(IRepository<ItemGroup> itemGroups)
    {
        _itemGroups = itemGroups;
    }

    public async Task<List<ItemGroupDto>> GetAllAsync()
    {
        var itemGroups = await _itemGroups.GetAllAsync();
        return itemGroups.Select(ToDto).ToList();
    }

    public async Task<ItemGroupDto?> GetByIdAsync(int id)
    {
        var itemGroup = await _itemGroups.GetByIdAsync(id);
        return itemGroup is null ? null : ToDto(itemGroup);
    }

    public async Task<ItemGroupDto> CreateAsync(CreateItemGroupDto dto)
    {
        var itemGroup = new ItemGroup
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ItemGroupcol = dto.ItemGroupcol
        };

        _itemGroups.Add(itemGroup);
        await _itemGroups.SaveChangesAsync();

        return ToDto(itemGroup);
    }

    public async Task<ItemGroupDto?> UpdateAsync(int id, UpdateItemGroupDto dto)
    {
        var itemGroup = await _itemGroups.GetByIdAsync(id);
        if (itemGroup is null) return null;

        itemGroup.Name = dto.Name;
        itemGroup.Description = dto.Description;
        itemGroup.UpdatedAt = DateTime.UtcNow;
        itemGroup.ItemGroupcol = dto.ItemGroupcol;
        _itemGroups.Update(itemGroup);

        await _itemGroups.SaveChangesAsync();

        return ToDto(itemGroup);
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        var itemGroup = await _itemGroups.GetByIdAsync(id);
        if (itemGroup is null) return null;

        _itemGroups.Delete(itemGroup);
        await _itemGroups.SaveChangesAsync();

        return true;
    }

    private static ItemGroupDto ToDto(ItemGroup itemGroup) => new()
    {
        Id = itemGroup.Id,
        Name = itemGroup.Name,
        Description = itemGroup.Description,
        CreatedAt = itemGroup.CreatedAt,
        UpdatedAt = itemGroup.UpdatedAt,
        ItemGroupcol = itemGroup.ItemGroupcol
    };
}