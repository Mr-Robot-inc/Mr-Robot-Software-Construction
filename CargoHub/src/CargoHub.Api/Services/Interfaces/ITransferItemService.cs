using CargoHub.Api.Dtos;

namespace CargoHub.Api.Services;

public interface ITransferItemService
{
    Task<List<TransferItemDto>> GetAllAsync();
    Task<TransferItemDto?> GetByIdAsync(int id);
    Task<List<TransferItemDto>> GetItemsForTransferAsync(int transferId);
    Task<TransferItemDto> CreateAsync(CreateTransferItemDto dto);
    Task<TransferItemDto?> UpdateAsync(int id, UpdateTransferItemDto dto);
    Task SetItemsForTransferAsync(int transferId, List<TransferItemDto> items);
    Task<bool?> DeleteAsync(int id);
    Task<bool?> RemoveItemsForTransferAsync(int transferId);
}
