using CargoHub.Api.Models;
using CargoHub.Api.Dtos;
public interface ISupplierService
{
    Task<SupplierDto> CreateAsync(CreateSupplierDto dto);
    Task<SupplierDto> GetSupplierByIdAsync(int id);
    Task<IEnumerable<SupplierDto>> GetAll();
    Task<SupplierDto> UpdateAsync(int id, UpdateSupplierDto dto);
    Task<bool> DeleteAsync(int id);
}