
using CargoHub.Api.Models;
using CargoHub.Api.Dtos;
using CargoHub.Api.Repositories;

namespace CargoHub.Api.Services;

public class SupplierService : ISupplierService
{
    private readonly IRepository<Supplier> _supplierRepository;

    public SupplierService(IRepository<Supplier> supplierRepository)
    {
        _supplierRepository = supplierRepository;
    }

    public async Task<IEnumerable<SupplierDto>> GetAll()
    {
        var suppliers = await _supplierRepository.GetAllAsync();
        return suppliers.Select(ToDto).ToList();
    }

    public async Task<SupplierDto> GetSupplierByIdAsync(int id)
    {
        var supplier = await _supplierRepository.GetByIdAsync(id);

        if (supplier == null)
        {
            throw new KeyNotFoundException(
                $"Supplier with ID {id} not found.");
        }

        return ToDto(supplier);
    }

    public async Task<SupplierDto> CreateAsync(CreateSupplierDto dto)
    {
        var supplier = new Supplier
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
            Reference = dto.Reference,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _supplierRepository.Add(supplier);
        await _supplierRepository.SaveChangesAsync();

        return ToDto(supplier);
    }

    public async Task<SupplierDto> UpdateAsync(int id, UpdateSupplierDto dto)
    {
        var existingSupplier =
            await _supplierRepository.GetByIdAsync(id);

        if (existingSupplier == null)
        {
            throw new KeyNotFoundException(
                $"Supplier with ID {id} not found.");
        }

        existingSupplier.Code = dto.Code;
        existingSupplier.Name = dto.Name;
        existingSupplier.Address = dto.Address;
        existingSupplier.City = dto.City;
        existingSupplier.ZipCode = dto.ZipCode;
        existingSupplier.Province = dto.Province;
        existingSupplier.Country = dto.Country;
        existingSupplier.ContactName = dto.ContactName;
        existingSupplier.ContactPhone = dto.ContactPhone;
        existingSupplier.Reference = dto.Reference;
        existingSupplier.UpdatedAt = DateTime.UtcNow;

        await _supplierRepository.SaveChangesAsync();

        return ToDto(existingSupplier);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var existingSupplier =
            await _supplierRepository.GetByIdAsync(id);

        if (existingSupplier == null)
        {
            return false;
        }

        _supplierRepository.Delete(existingSupplier);
        await _supplierRepository.SaveChangesAsync();

        return true;
    }

    private static SupplierDto ToDto(Supplier supplier)
    {
        return new SupplierDto
        {
            Id = supplier.Id,
            Code = supplier.Code,
            Name = supplier.Name,
            Address = supplier.Address,
            City = supplier.City,
            ZipCode = supplier.ZipCode,
            Province = supplier.Province,
            Country = supplier.Country,
            ContactName = supplier.ContactName,
            ContactPhone = supplier.ContactPhone,
            Reference = supplier.Reference,
            CreatedAt = supplier.CreatedAt,
            UpdatedAt = supplier.UpdatedAt
        };
    }
}
