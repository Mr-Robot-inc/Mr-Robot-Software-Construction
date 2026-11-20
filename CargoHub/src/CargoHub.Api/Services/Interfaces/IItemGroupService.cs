using CargoHub.Api.Dtos;

namespace CargoHub.Api.Services;

public interface IItemGroupService
{
    Task<List<ItemGroupDto>> GetAllAsync();
    Task<ItemGroupDto?> GetByIdAsync(int id);
    Task<ItemGroupDto> CreateAsync(CreateItemGroupDto dto);
    Task<ItemGroupDto?> UpdateAsync(int id, UpdateItemGroupDto dto);
    Task<bool?> DeleteAsync(int id);
}
