using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;
using CargoHub.Api.Services;

namespace CargoHub.Api.Services;

public class ItemService : IItemService
{
    private readonly IRepository<Item> _items;

    public ItemService(IRepository<Item> items)
    {
        _items = items;
    }

    public async Task<List<ItemDto>> GetAllAsync()
    {
        var items = await _items.GetAllAsync();
        return items.Select(ToDto).ToList();
    }

    public async Task<ItemDto?> GetByIdAsync(int id)
    {
        var item = await _items.GetByIdAsync(id);
        return item is null ? null : ToDto(item);
    }

    public async Task<List<ItemDto?>> GetItemsByItemLineIdAsync(int id)
    {
        return null;
    }

    public async Task<List<ItemDto?>> GetItemsByItemGroupIdAsync(int id)
    {
        return null;
    }

    public async Task<List<ItemDto?>> GetItemsByItemTypeIdAsync(int id)
    {
        return null;
    }

    public async Task<List<ItemDto?>> GetItemsBySupplierIdAsync(int id)
    {
        return null;
    }

    public async Task<ItemDto> CreateAsync(CreateItemDto dto)
    {
        var item = new Item
        {
            Code = dto.Code,
            Description = dto.Description,
            BarCode = dto.BarCode,
            ModeNumber = dto.ModeNumber,
            CommodityCode = dto.CommodityCode,
            UnitWeight = dto.UnitWeight,
            ItemLineId = dto.ItemLineId,
            ItemGroupId = dto.ItemGroupId,
            ItemTypeId = dto.ItemTypeId,
            MinPurchaseQty = dto.MinPurchaseQty,
            CaseSize = dto.CaseSize,
            PackagingType = dto.PackagingType,
            OrderMultiple = dto.OrderMultiple,
            SupplierId = dto.SupplierId,
            SupplierSku = dto.SupplierSku,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _items.Add(item);
        await _items.SaveChangesAsync();

        return ToDto(item);
    }

    public async Task<ItemDto?> UpdateAsync(int id, UpdateItemDto dto)
    {
        var item = await _items.GetByIdAsync(id);
        if (item is null) return null;

        item.Code = dto.Code;
        item.Description = dto.Description;
        item.BarCode = dto.BarCode;
        item.ModeNumber = dto.ModeNumber;
        item.CommodityCode = dto.CommodityCode;
        item.UnitWeight = dto.UnitWeight;
        item.ItemLineId = dto.ItemLineId;
        item.ItemGroupId = dto.ItemGroupId;
        item.ItemTypeId = dto.ItemTypeId;
        item.MinPurchaseQty = dto.MinPurchaseQty;
        item.CaseSize = dto.CaseSize;
        item.PackagingType = dto.PackagingType;
        item.OrderMultiple = dto.OrderMultiple;
        item.SupplierId = dto.SupplierId;
        item.SupplierSku = dto.SupplierSku;
        item.UpdatedAt = DateTime.UtcNow;

        await _items.SaveChangesAsync();

        return ToDto(item);
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        var item = await _items.GetByIdAsync(id);
        if (item is null) return null;

        _items.Delete(item);
        await _items.SaveChangesAsync();

        return true;
    }


    private static ItemDto ToDto(Item item) => new()
    {
            Code = item.Code,
            Description = item.Description,
            BarCode = item.BarCode,
            ModeNumber = item.ModeNumber,
            CommodityCode = item.CommodityCode,
            UnitWeight = item.UnitWeight,
            ItemLineId = item.ItemLineId,
            ItemGroupId = item.ItemGroupId,
            ItemTypeId = item.ItemTypeId,
            MinPurchaseQty = item.MinPurchaseQty,
            CaseSize = item.CaseSize,
            PackagingType = item.PackagingType,
            OrderMultiple = item.OrderMultiple,
            SupplierId = item.SupplierId,
            SupplierSku = item.SupplierSku,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
    };
}