using CargoHub.Api.Dtos;

namespace CargoHub.Api.Services;

public interface IItemService
{
    Task<List<ItemDto>> GetAllAsync();
    Task<ItemDto?> GetByIdAsync(int id);
    Task<List<ItemDto?>> GetItemsByItemLineIdAsync(int id);
    Task<List<ItemDto?>> GetItemsByItemGroupIdAsync(int id);
    Task<List<ItemDto?>> GetItemsByItemTypeIdAsync(int id);
    Task<List<ItemDto?>> GetItemsBySupplierIdAsync(int id);
    Task<ItemDto> CreateAsync(CreateItemDto dto);
    Task<ItemDto?> UpdateAsync(int id, UpdateItemDto dto);
    Task<bool?> DeleteAsync(int id);
}