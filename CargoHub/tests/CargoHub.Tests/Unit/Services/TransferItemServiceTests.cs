using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Services;
using CargoHub.Tests.Unit.Fakes;

namespace CargoHub.Tests.Unit.Services;

public class TransferItemServiceTests
{
    private readonly FakeRepository<TransferItem> _transferItems = new();
    private readonly FakeRepository<Transfer> _transfers = new();
    private readonly FakeRepository<Item> _items = new();
    private readonly TransferItemService _service;

    public TransferItemServiceTests()
    {
        _service = new TransferItemService(_transferItems, _transfers, _items);
        _transfers.Add(new Transfer { Reference = "T1", FromLocationId = 1, ToLocationId = 2, TransferStatus = "Scheduled" });
        _transfers.Add(new Transfer { Reference = "T2", FromLocationId = 1, ToLocationId = 2, TransferStatus = "Scheduled" });
        _items.Add(new Item());
        _items.Add(new Item());
    }

    [Fact]
    public async Task GetItemsForTransferAsync_ReturnsOnlyItemsOfThatTransfer()
    {
        _transferItems.Add(new TransferItem { TransferId = 1, ItemId = 1, Amount = 5 });
        _transferItems.Add(new TransferItem { TransferId = 2, ItemId = 2, Amount = 7 });

        var result = await _service.GetItemsForTransferAsync(1);

        Assert.Single(result);
        Assert.Equal(1, result[0].TransferId);
        Assert.Equal(5, result[0].Amount);
    }

    [Fact]
    public async Task GetItemsForTransferAsync_ReturnsEmpty_WhenNoItems()
    {
        Assert.Empty(await _service.GetItemsForTransferAsync(1));
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenMissing()
    {
        Assert.Null(await _service.GetByIdAsync(99));
    }

    [Fact]
    public async Task CreateAsync_CreatesTransferItem()
    {
        var result = await _service.CreateAsync(new CreateTransferItemDto { TransferId = 1, ItemId = 2, Amount = 3 });

        Assert.Equal(1, result.TransferId);
        Assert.Equal(2, result.ItemId);
        Assert.Equal(3, result.Amount);
        Assert.Single(_transferItems.Items);
        Assert.Equal(1, _transferItems.SaveCount);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenTransferDoesNotExist()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(new CreateTransferItemDto { TransferId = 99, ItemId = 1, Amount = 3 }));
        Assert.Empty(_transferItems.Items);
    }

    [Fact]
    public async Task CreateAsync_Throws_WhenItemDoesNotExist()
    {
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.CreateAsync(new CreateTransferItemDto { TransferId = 1, ItemId = 99, Amount = 3 }));
        Assert.Empty(_transferItems.Items);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingTransferItem()
    {
        _transferItems.Add(new TransferItem { TransferId = 1, ItemId = 1, Amount = 5 });

        var result = await _service.UpdateAsync(1, new UpdateTransferItemDto { TransferId = 2, ItemId = 2, Amount = 9 });

        Assert.NotNull(result);
        Assert.Equal(2, result.TransferId);
        Assert.Equal(2, result.ItemId);
        Assert.Equal(9, result.Amount);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenMissing()
    {
        var result = await _service.UpdateAsync(99, new UpdateTransferItemDto { TransferId = 1, ItemId = 1, Amount = 1 });
        Assert.Null(result);
        Assert.Equal(0, _transferItems.SaveCount);
    }

    [Fact]
    public async Task SetItemsForTransferAsync_ReplacesAllItemsOfTheTransfer()
    {
        _transferItems.Add(new TransferItem { TransferId = 1, ItemId = 1, Amount = 5 });
        _transferItems.Add(new TransferItem { TransferId = 2, ItemId = 1, Amount = 8 });

        await _service.SetItemsForTransferAsync(1, new List<TransferItemDto>
        {
            new() { ItemId = 2, Amount = 4 },
            new() { ItemId = 1, Amount = 6 }
        });

        var forTransfer1 = _transferItems.Items.Where(i => i.TransferId == 1).ToList();
        Assert.Equal(2, forTransfer1.Count);
        Assert.Contains(forTransfer1, i => i.ItemId == 2 && i.Amount == 4);
        Assert.Contains(forTransfer1, i => i.ItemId == 1 && i.Amount == 6);
        Assert.Single(_transferItems.Items.Where(i => i.TransferId == 2));
    }

    [Fact]
    public async Task SetItemsForTransferAsync_Throws_KeyNotFound_WhenTransferMissing()
    {
        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _service.SetItemsForTransferAsync(99, new List<TransferItemDto>()));
    }

    [Fact]
    public async Task SetItemsForTransferAsync_Throws_WhenItemMissing_AndKeepsOldItems()
    {
        _transferItems.Add(new TransferItem { TransferId = 1, ItemId = 1, Amount = 5 });

        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.SetItemsForTransferAsync(1, new List<TransferItemDto> { new() { ItemId = 99, Amount = 1 } }));

        Assert.Single(_transferItems.Items);
    }

    [Fact]
    public async Task DeleteAsync_DeletesExistingTransferItem()
    {
        _transferItems.Add(new TransferItem { TransferId = 1, ItemId = 1, Amount = 5 });

        Assert.True(await _service.DeleteAsync(1));
        Assert.Empty(_transferItems.Items);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNull_WhenMissing()
    {
        Assert.Null(await _service.DeleteAsync(99));
    }

    [Fact]
    public async Task RemoveItemsForTransferAsync_RemovesOnlyThatTransfersItems()
    {
        _transferItems.Add(new TransferItem { TransferId = 1, ItemId = 1, Amount = 5 });
        _transferItems.Add(new TransferItem { TransferId = 1, ItemId = 2, Amount = 5 });
        _transferItems.Add(new TransferItem { TransferId = 2, ItemId = 1, Amount = 5 });

        Assert.True(await _service.RemoveItemsForTransferAsync(1));
        Assert.Single(_transferItems.Items);
        Assert.Equal(2, _transferItems.Items[0].TransferId);
    }

    [Fact]
    public async Task RemoveItemsForTransferAsync_ReturnsNull_WhenTransferMissing()
    {
        Assert.Null(await _service.RemoveItemsForTransferAsync(99));
    }
}
