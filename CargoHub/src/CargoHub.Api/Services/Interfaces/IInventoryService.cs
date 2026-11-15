using CargoHub.Api.Dtos;

namespace CargoHub.Api.Services;

public interface IInventoryService
{
    Task<List<InventoryDto>> GetAllAsync();
    Task<InventoryDto?> GetByIdAsync(int id);
    Task<List<InventoryDto>?> GetByItemIdAsync(int id);
    Task<List<int>?> GetTotalsByItemId(int id);
    
    Task<InventoryDto> CreateAsync(CreateInventoryDto dto);
    Task<InventoryDto?> UpdateAsync(int id, UpdateInventoryDto dto);
    Task<bool?> DeleteAsync(int id);
}