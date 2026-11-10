using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;
using CargoHub.Api.Services;

namespace CargoHub.Api.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IRepository<Warehouse> _warehouses;

    public WarehouseService(IRepository<Warehouse> warehouses) 
    {
                _warehouses = warehouses;
    }
    public async Task<List<WarehouseDto>> GetAllAsync() 
    {
        var warehouses = await _warehouses.GetAllAsync();
        return warehouses.Select(ToDto).ToList();
    }
    public async Task<WarehouseDto?> GetByIdAsync(int id)
    {
        var warehouse = await _warehouses.GetByIdAsync(id);
        return warehouse is null ? null : ToDto(warehouse);
    }
    public async Task<WarehouseDto> CreateAsync(CreateWarehouseDto dto)
    {
        var warehouse = new Warehouse
        {
            Code = dto.Code,
            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            ZipCode = dto.ZipCode,
            Province = dto.Province,
            Country = dto.Country,
            ContactName = dto.ContactName,
            ContactPhone = dto.ContactPhone,
            ContactEmail = dto.ContactEmail,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _warehouses.Add(warehouse);
        await _warehouses.SaveChangesAsync();
        return ToDto(warehouse);
    }
    public async Task<WarehouseDto?> UpdateAsync(int id, UpdatedWarehouseDto dto)
    {
        var warehouse = await _warehouses.GetByIdAsync(id);
        if (warehouse is null) return null;
        warehouse.Code = dto.Code;
        warehouse.Name = dto.Name;
        warehouse.Address = dto.Address;
        warehouse.City = dto.City;
        warehouse.ZipCode = dto.ZipCode;
        warehouse.Province = dto.Province;
        warehouse.Country = dto.Country;
        warehouse.ContactName = dto.ContactName;
        warehouse.ContactPhone = dto.ContactPhone;
        warehouse.ContactEmail = dto.ContactEmail;
        warehouse.UpdatedAt = DateTime.UtcNow;

        _warehouses.Update(warehouse);
        await _warehouses.SaveChangesAsync();
        return ToDto(warehouse);
    }
    public async Task<bool?> DeleteAsync(int id)
    {
        var warehouse = await _warehouses.GetByIdAsync(id);
        if (warehouse is null) return null;

        _warehouses.Delete(warehouse);
        await _warehouses.SaveChangesAsync();
        return true;
    }
    private static WarehouseDto ToDto(Warehouse warehouse)
    {
        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Address = warehouse.Address,
            City = warehouse.City,
            ZipCode = warehouse.ZipCode,
            Province = warehouse.Province,
            Country = warehouse.Country,
            ContactName = warehouse.ContactName,
            ContactPhone = warehouse.ContactPhone,
            ContactEmail = warehouse.ContactEmail,
            CreatedAt = warehouse.CreatedAt,
            UpdatedAt = warehouse.UpdatedAt
        };
    }
}