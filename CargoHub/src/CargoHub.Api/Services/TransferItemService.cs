using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;

namespace CargoHub.Api.Services;

public class TransferItemService : ITransferItemService
{
    private readonly IRepository<TransferItem> _transferItems;
    private readonly IRepository<Transfer> _transfers;
    private readonly IRepository<Item> _items;

    public TransferItemService(IRepository<TransferItem> transferItems, IRepository<Transfer> transfers, IRepository<Item> items)
    {
        _transferItems = transferItems;
        _transfers = transfers;
        _items = items;
    }

    public async Task<List<TransferItemDto>> GetAllAsync()
    {
        var transferItems = await _transferItems.GetAllAsync();
        return transferItems.Select(ToDto).ToList();
    }

    public async Task<TransferItemDto?> GetByIdAsync(int id)
    {
        var transferItem = await _transferItems.GetByIdAsync(id);
        return transferItem is null ? null : ToDto(transferItem);
    }

    public async Task<List<TransferItemDto>> GetItemsForTransferAsync(int transferId)
    {
        var items = await _transferItems.FindAsync(i => i.TransferId == transferId);
        return items.Select(ToDto).ToList();
    }

    public async Task<TransferItemDto> CreateAsync(CreateTransferItemDto dto)
    {
        await DoesTransferExistAsync(dto.TransferId);
        await DoItemsExistAsync(new List<int> { dto.ItemId });

        var transferItem = new TransferItem
        {
            TransferId = dto.TransferId,
            ItemId = dto.ItemId,
            Amount = dto.Amount
        };

        _transferItems.Add(transferItem);
        await _transferItems.SaveChangesAsync();

        return ToDto(transferItem);
    }

    public async Task<TransferItemDto?> UpdateAsync(int id, UpdateTransferItemDto dto)
    {
        var transferItem = await _transferItems.GetByIdAsync(id);
        if (transferItem is null) return null;

        await DoesTransferExistAsync(dto.TransferId);
        await DoItemsExistAsync(new List<int> { dto.ItemId });

        transferItem.TransferId = dto.TransferId;
        transferItem.ItemId = dto.ItemId;
        transferItem.Amount = dto.Amount;

        await _transferItems.SaveChangesAsync();

        return ToDto(transferItem);
    }

    public async Task SetItemsForTransferAsync(int transferId, List<TransferItemDto> items)
    {
        await DoesTransferExistAsync(transferId, throwNotFound: true);
        await DoItemsExistAsync(items.Select(i => i.ItemId).ToList());

        var currentItems = await _transferItems.FindAsync(i => i.TransferId == transferId);
        foreach (var item in currentItems) _transferItems.Delete(item);

        foreach (var item in items)
        {
            _transferItems.Add(new TransferItem
            {
                TransferId = transferId,
                ItemId = item.ItemId,
                Amount = item.Amount
            });
        }

        await _transferItems.SaveChangesAsync();
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        var transferItem = await _transferItems.GetByIdAsync(id);
        if (transferItem is null) return null;

        _transferItems.Delete(transferItem);
        await _transferItems.SaveChangesAsync();

        return true;
    }

    public async Task<bool?> RemoveItemsForTransferAsync(int transferId)
    {
        if (await _transfers.GetByIdAsync(transferId) is null) return null;

        var items = await _transferItems.FindAsync(i => i.TransferId == transferId);
        foreach (var item in items) _transferItems.Delete(item);

        await _transferItems.SaveChangesAsync();

        return true;
    }

    private async Task DoesTransferExistAsync(int transferId, bool throwNotFound = false)
    {
        if (await _transfers.GetByIdAsync(transferId) is not null) return;

        if (throwNotFound) throw new KeyNotFoundException($"Transfer {transferId} was not found.");
        throw new ArgumentException($"Transfer {transferId} does not exist.", nameof(transferId));
    }

    private async Task DoItemsExistAsync(List<int> itemIds)
    {
        var ids = itemIds.Distinct().ToList();
        if (ids.Count == 0) return;

        var found = (await _items.FindAsync(i => ids.Contains(i.Id))).Select(i => i.Id).ToHashSet();
        var missing = ids.Where(id => !found.Contains(id)).Cast<int?>().FirstOrDefault();
        if (missing is not null)
            throw new ArgumentException($"Item {missing} does not exist.");
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
