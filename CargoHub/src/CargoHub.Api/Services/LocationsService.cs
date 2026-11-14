
using CargoHub.Api.Models;
using CargoHub.Api.Dtos;
using CargoHub.Api.Repositories;

namespace CargoHub.Api.Services;

public class LocationsService : ILocationsService
{
	private readonly IRepository<Location> _locations;
	private readonly IRepository<Warehouse> _warehouses;

	public LocationsService(
		IRepository<Location> locations,
		IRepository<Warehouse> warehouses)
	{
		_locations = locations;
		_warehouses = warehouses;
	}

	public async Task<List<LocationDto>> GetAllAsync()
	{
		var locations = await _locations.GetAllAsync();
		return locations.Select(ToDto).ToList();
	}

	public async Task<LocationDto?> GetByIdAsync(int id)
	{
		var location = await _locations.GetByIdAsync(id);
		return location is null ? null : ToDto(location);
	}

	public async Task<LocationDto> CreateAsync(CreateLocationDto dto)
	{
		if (dto.WarehouseId <= 0)
		{
			throw new ArgumentException(
				"WarehouseId must be greater than zero.");
		}

		// Check the Warehouses table, not the Locations table
		var warehouse = await _warehouses.GetByIdAsync(dto.WarehouseId);

		if (warehouse is null)
		{
			throw new ArgumentException(
				$"Warehouse with ID {dto.WarehouseId} does not exist.");
		}

		var location = new Location
		{
			WarehouseId = dto.WarehouseId,
			Code = dto.Code,
			Name = dto.Name,
			CreatedAt = DateTime.UtcNow,
			UpdatedAt = DateTime.UtcNow
		};

		_locations.Add(location);
		await _locations.SaveChangesAsync();

		return ToDto(location);
	}

	public async Task<LocationDto?> UpdateAsync(
		int id, UpdatedLocationDto dto)
	{
		var location = await _locations.GetByIdAsync(id);

		if (location is null)
			return null;

		if (dto.WarehouseId <= 0)
		{
			throw new ArgumentException(
				"WarehouseId must be greater than zero.");
		}

		// Check the Warehouses table
		var warehouse = await _warehouses.GetByIdAsync(dto.WarehouseId);

		if (warehouse is null)
		{
			throw new ArgumentException(
				$"Warehouse with ID {dto.WarehouseId} does not exist.");
		}

		location.WarehouseId = dto.WarehouseId;
		location.Code = dto.Code;
		location.Name = dto.Name;
		location.UpdatedAt = DateTime.UtcNow;

		await _locations.SaveChangesAsync();

		return ToDto(location);
	}

	public async Task<bool?> DeleteAsync(int id)
	{
		var location = await _locations.GetByIdAsync(id);

		if (location is null)
			return null;

		_locations.Delete(location);
		await _locations.SaveChangesAsync();

		return true;
	}

	public static LocationDto ToDto(Location location)
	{
		return new LocationDto
		{
			Id = location.Id,
			WarehouseId = location.WarehouseId,
			Code = location.Code,
			Name = location.Name,
			CreatedAt = location.CreatedAt,
			UpdatedAt = location.UpdatedAt
		};
	}
}
