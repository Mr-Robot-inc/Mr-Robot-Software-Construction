using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;

namespace CargoHub.Api.Services;

public class ItemLineService : IItemLineService
{
    private readonly IRepository<ItemLine> _itemLine;

    public ItemLineService(IRepository<ItemLine> itemLines)
    {
        _itemLine = itemLines;
    }

    public async Task<List<ItemLineDto>> GetAllAsync()
    {
        var itemLines = await _itemLine.GetAllAsync();
        return itemLines.Select(ToDto).ToList();
    }

    public async Task<ItemLineDto?> GetByIdAsync(int id)
    {
        var itemLine = await _itemLine.GetByIdAsync(id);
        return itemLine is null ? null : ToDto(itemLine);
    }

    public async Task<ItemLineDto> CreateAsync(CreateItemLineDto dto)
    {
        var itemLine = new ItemLine
        {
            Name = dto.Name,
            Description = dto.Description,
        };

        _itemLine.Add(itemLine);
        await _itemLine.SaveChangesAsync();

        return ToDto(itemLine);
    }

    public async Task<ItemLineDto?> UpdateAsync(int id, UpdateItemLineDto dto)
    {
        var itemLine = await _itemLine.GetByIdAsync(id);
        if (itemLine is null) return null;

        itemLine.Name = dto.Name;
        itemLine.Description = dto.Description;
        _itemLine.Update(itemLine);

        await _itemLine.SaveChangesAsync();

        return ToDto(itemLine);
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        var itemLine = await _itemLine.GetByIdAsync(id);
        if (itemLine is null) return false;

        _itemLine.Delete(itemLine);
        await _itemLine.SaveChangesAsync();

        return true;
    }

    private static ItemLineDto ToDto(ItemLine itemLine) => new()
    {
        Id = itemLine.Id,
        Name = itemLine.Name,
        Description = itemLine.Description,
        CreatedAt = itemLine.CreatedAt,
        UpdatedAt = itemLine.UpdatedAt
    };
}
