using CargoHub.Api.Models;
using CargoHub.Api.Dtos;
using CargoHub.Api.Repositories;

namespace CargoHub.Api.Services;

public class TransferItemService : ITransferItemService
{
    private readonly IRepository<TransferItem> _transferItemRepository;
    public TransferItemService(IRepository<TransferItem> repository)
    {
        _transferItemRepository = repository;
    }

    public async Task<List<TransferItemDto>> GetAllAsync()
    {
        var transferItems = await _transferItemRepository.GetAllAsync();
        return transferItems.Select(ToDto).ToList();
    }

    public async Task<TransferItemDto?> GetByIdAsync(int id)
    {
        var transferItem = await _transferItemRepository.GetByIdAsync(id);
        return transferItem is null ? null : ToDto(transferItem);
    }

    public async Task<TransferItemDto> CreateAsync(CreateTransferItemDto dto)
    {
        var transferItem = new TransferItem
        {
            TransferId = dto.TransferId,
            ItemId = dto.ItemId,
            Amount = dto.Amount
        };

        _transferItemRepository.Add(transferItem);
        await _transferItemRepository.SaveChangesAsync();

        return ToDto(transferItem);
    }

    public async Task<TransferItemDto?> UpdateAsync(int id, UpdateTransferItemDto dto)
    {
        var transferItem = await _transferItemRepository.GetByIdAsync(id);
        if (transferItem is null) return null;

        transferItem.TransferId = dto.TransferId;
        transferItem.ItemId = dto.ItemId;
        transferItem.Amount = dto.Amount;

        _transferItemRepository.Update(transferItem);
        await _transferItemRepository.SaveChangesAsync();

        return ToDto(transferItem);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var transferItem = await _transferItemRepository.GetByIdAsync(id);
        if (transferItem is null) return false;

        _transferItemRepository.Delete(transferItem);
        await _transferItemRepository.SaveChangesAsync();

        return true;
    }

    private static TransferItemDto ToDto(TransferItem transferItem)
    {
        return new TransferItemDto
        {
            Id = transferItem.Id,
            TransferId = transferItem.TransferId,
            ItemId = transferItem.ItemId,
            Amount = transferItem.Amount
        };
    }
}
