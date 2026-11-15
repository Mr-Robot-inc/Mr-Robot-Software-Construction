using CargoHub.Api.Dtos;

namespace CargoHub.Api.Services;

public interface ITransferService
{
    Task<List<TransferDto>> GetAllAsync();
    Task<TransferDto?> GetByIdAsync(int id);
    Task<List<TransferItemDto>> GetItemsInTransferAsync(int transferId);
    Task<TransferDto> CreateAsync(CreateTransferDto dto);
    Task<TransferDto?> UpdateAsync(int id, UpdateTransferDto dto);
    Task<bool?> DeleteAsync(int id);
}