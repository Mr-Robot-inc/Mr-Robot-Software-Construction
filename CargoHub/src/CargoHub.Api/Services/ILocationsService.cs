using CargoHub.Api.Models;
using CargoHub.Api.Dtos;

namespace CargoHub.Api.Services;

public interface ILocationsService
{
	Task<List<LocationDto>> GetAllAsync();
	Task<LocationDto?> GetByIdAsync(int id);
	Task<LocationDto> CreateAsync(CreateLocationDto dto);
	Task<LocationDto?> UpdateAsync(int id, UpdatedLocationDto dto);
	Task<bool?> DeleteAsync(int id);
}