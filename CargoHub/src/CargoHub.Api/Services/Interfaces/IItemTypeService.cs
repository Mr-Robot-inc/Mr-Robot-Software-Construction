using CargoHub.Api.Dtos;

namespace CargoHub.Api.Services;

public interface IItemTypeService
{
    Task<List<ItemTypeDto>> GetAllAsync();
    Task<ItemTypeDto?> GetByIdAsync(int id);
    Task<ItemTypeDto> CreateAsync(CreateItemTypeDto dto);
    Task<ItemTypeDto?> UpdateAsync(int id, UpdateItemTypeDto dto);
    Task<bool?> DeleteAsync(int id);
}
