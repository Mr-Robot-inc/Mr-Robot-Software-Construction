using CargoHub.Api.Dtos;

namespace CargoHub.Api.Services;

public interface ITransferItemService
{
    Task<TransferItemDto> CreateAsync(CreateTransferItemDto dto);
    Task<TransferItemDto?> GetByIdAsync(int id);
    Task<List<TransferItemDto>> GetAllAsync();
    Task<TransferItemDto?> UpdateAsync(int id, UpdateTransferItemDto dto);
    Task<bool> DeleteAsync(int id);
}   
