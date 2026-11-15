using CargoHub.Api.Dtos;
using CargoHub.Api.Models;

namespace CargoHub.Api.Services;

public interface IWarehouseService
{
    Task<List<WarehouseDto>> GetAllAsync();
    Task<WarehouseDto?> GetByIdAsync(int id);
    Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto);
    Task<WarehouseDto?> UpdateAsync(int id, UpdatedWarehouseDto dto);
    Task<bool?> DeleteAsync(int id);
}