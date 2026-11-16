using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;

namespace CargoHub.Api.Services;

public class ItemTypeService : IItemTypeService
{
    private readonly IRepository<ItemType> _itemTypes;

    public ItemTypeService(IRepository<ItemType> itemTypes)
    {
        _itemTypes = itemTypes;
    }

    public async Task<List<ItemTypeDto>> GetAllAsync()
    {
        var itemTypes = await _itemTypes.GetAllAsync();
        return itemTypes.Select(ToDto).ToList();
    }

    public async Task<ItemTypeDto?> GetByIdAsync(int id)
    {
        var itemType = await _itemTypes.GetByIdAsync(id);
        return itemType is null ? null : ToDto(itemType);
    }

    public async Task<ItemTypeDto> CreateAsync(CreateItemTypeDto dto)
    {
        var itemType = new ItemType
        {
            Name = dto.Name,
            Description = dto.Description,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _itemTypes.Add(itemType);
        await _itemTypes.SaveChangesAsync();
        
        return ToDto(itemType);
    }

    public async Task<ItemTypeDto?> UpdateAsync(int id, UpdateItemTypeDto dto)
    {
        var itemType = await _itemTypes.GetByIdAsync(id);
        if (itemType is null) return null;

        itemType.Name = dto.Name;
        itemType.Description = dto.Description;
        itemType.UpdatedAt = DateTime.UtcNow;
        _itemTypes.Update(itemType);

        await _itemTypes.SaveChangesAsync();

        return ToDto(itemType);
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        var itemType = await _itemTypes.GetByIdAsync(id);
        if (itemType is null) return null;

        _itemTypes.Delete(itemType);
        await _itemTypes.SaveChangesAsync();

        return true;
    }

    private static ItemTypeDto ToDto(ItemType itemType) => new()
    {
        Id = itemType.Id,
        Name = itemType.Name,
        Description = itemType.Description,
        CreatedAt = itemType.CreatedAt,
        UpdatedAt = itemType.UpdatedAt
    };
}
