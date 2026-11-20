using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;

namespace CargoHub.Api.Services;

public class InventoryService : IInventoryService
{
    private readonly IRepository<Inventory> _inventories;
    private readonly IRepository<Item> _items;
    private readonly IRepository<Location> _locations;

    public InventoryService(IRepository<Inventory> inventories, IRepository<Item> items, IRepository<Location> locations)
    {
        _inventories = inventories;
        _items = items;
        _locations = locations;
        
    }

    public async Task<List<InventoryDto>> GetAllAsync()
    {
        var inventories = await _inventories.GetAllAsync();
        return inventories.Select(ToDto).ToList();
    }

    public async Task<InventoryDto?> GetByIdAsync(int id)
    {
        var inventory = await _inventories.GetByIdAsync(id);
        return inventory is null ? null : ToDto(inventory);
    }

    public async Task<List<InventoryDto>?> GetByItemIdAsync(int id)
    {
        return null;
    }
    public async Task<List<int>?> GetTotalsByItemId(int id)
    {
        return null;
    }

    public async Task<InventoryDto?> CreateAsync(CreateInventoryDto dto)
    {
        var location = await _locations.GetByIdAsync(dto.LocationId);
        var item = await _items.GetByIdAsync(dto.ItemId);

        if (location == null || item == null) return null;

        var inventory = new Inventory
        {
            ItemId = dto.ItemId,
            LocationId = dto.LocationId,
            QuantityOnHand = dto.QuantityOnHand,
            QuantityOrdered = dto.QuantityOrdered,
            QuantityAllocated = dto.QuantityAllocated,
        };

        _inventories.Add(inventory);
        await _inventories.SaveChangesAsync();

        return ToDto(inventory);
    }

    public async Task<InventoryDto?> UpdateAsync(int id, UpdateInventoryDto dto)
    {
        var inventory = await _inventories.GetByIdAsync(id);
        if (inventory is null) return null;

        inventory.ItemId = dto.ItemId;
        inventory.LocationId = dto.LocationId;
        inventory.QuantityOnHand = dto.QuantityOnHand;
        inventory.QuantityOrdered = dto.QuantityOrdered;
        inventory.QuantityAllocated = dto.QuantityAllocated;

        await _inventories.SaveChangesAsync();

        return ToDto(inventory);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var inventory = await _inventories.GetByIdAsync(id);
        if (inventory is null) return false;

        _inventories.Delete(inventory);
        await _inventories.SaveChangesAsync();

        return true;
    }

    private static InventoryDto ToDto(Inventory inventory) => new()
    {
        Id = inventory.Id,
        ItemId = inventory.ItemId,
        LocationId = inventory.LocationId,
        QuantityOnHand = inventory.QuantityOnHand,
        QuantityOrdered = inventory.QuantityOrdered,
        QuantityAllocated = inventory.QuantityAllocated,
        CreatedAt = inventory.CreatedAt,
        UpdatedAt = inventory.UpdatedAt
    };
}