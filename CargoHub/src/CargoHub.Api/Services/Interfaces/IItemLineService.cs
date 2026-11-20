using CargoHub.Api.Dtos;

namespace CargoHub.Api.Services;

public interface IItemLineService
{
    Task<List<ItemLineDto>> GetAllAsync();
    Task<ItemLineDto?> GetByIdAsync(int id);
    Task<ItemLineDto> CreateAsync(CreateItemLineDto dto);
    Task<ItemLineDto?> UpdateAsync(int id, UpdateItemLineDto dto);
    Task<bool?> DeleteAsync(int id);
}
