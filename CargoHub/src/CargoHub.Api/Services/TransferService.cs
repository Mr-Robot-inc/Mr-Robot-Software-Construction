using CargoHub.Api.Repositories;
using CargoHub.Api.Models;
using CargoHub.Api.Dtos;

namespace CargoHub.Api.Services;

public class TransferService : ITransferService
{
    private readonly IRepository<Transfer> _transfers;
    private readonly IRepository<TransferItem> _transferItems;

    public TransferService(IRepository<Transfer> transfers, IRepository<TransferItem> transferItems)
    {
        _transfers = transfers;
        _transferItems = transferItems;
    }
    public async Task<List<TransferDto>> GetAllAsync()
    {
        var transfers = await _transfers.GetAllAsync();
        return transfers.Select(ToDto).ToList();
    }

    public async Task<TransferDto?> GetByIdAsync(int id)
    {
        var transfer = await _transfers.GetByIdAsync(id);
        return transfer is null ? null : ToDto(transfer);
    }

    public async Task<List<TransferItemDto>> GetItemsInTransferAsync(int transferId)
    {
        var items = await _transferItems.GetAllAsync();
        return items
            .Where(i => i.TransferId == transferId)
            .Select(ToItemDto)
            .ToList();
    }

    public async Task<TransferDto> CreateAsync(CreateTransferDto dto)
    {
        var date = DateTime.UtcNow;

        var transfer = new Transfer
        {
            Reference = dto.Reference,
            ToLocationId = dto.ToLocationId,
            FromLocationId = dto.FromLocationId,
            TransferStatus = "Scheduled",
            CreatedAt = date,
            UpdatedAt = date
        };

        _transfers.Add(transfer);
        await _transfers.SaveChangesAsync();

        return ToDto(transfer);
    }

    public async Task<TransferDto?> UpdateAsync(int id, UpdateTransferDto dto)
    {
        var transfer = await _transfers.GetByIdAsync(id);

        if (transfer is null) return null;
        transfer.Reference = dto.Reference;
        transfer.ToLocationId = dto.ToLocationId;
        transfer.FromLocationId = dto.FromLocationId;
        transfer.TransferStatus = dto.TransferStatus;
        transfer.UpdatedAt = DateTime.UtcNow;

        await _transfers.SaveChangesAsync();

        return ToDto(transfer);
    }

    public async Task<bool?> DeleteAsync(int id)
    {
        var transfer = await _transfers.GetByIdAsync(id);

        if (transfer is null) return null;
        _transfers.Delete(transfer);
        await _transfers.SaveChangesAsync();

        return true;
    }

    private static TransferDto ToDto(Transfer transfer)
    {
        return new TransferDto
        {
            Id = transfer.Id,
            Reference = transfer.Reference,
            ToLocationId = transfer.ToLocationId,
            FromLocationId = transfer.FromLocationId,
            TransferStatus = transfer.TransferStatus,
            CreatedAt = transfer.CreatedAt,
            UpdatedAt = transfer.UpdatedAt
        };
    }

    private static TransferItemDto ToItemDto(TransferItem item)
    {
        return new TransferItemDto
        {
            Id = item.Id,
            TransferId = item.TransferId,
            ItemId = item.ItemId,
            Amount = item.Amount
        };
    }
}