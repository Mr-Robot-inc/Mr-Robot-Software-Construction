using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;
using CargoHub.Api.Services;

namespace CargoHub.Tests.Services;

public class TransferServiceTests
{
    private readonly TestTransferRepository _repository;
    private readonly TestTransferItemRepository _itemRepository;
    private readonly TransferService _service;
    public TransferServiceTests()
    {
        _repository = new TestTransferRepository();
        _itemRepository = new TestTransferItemRepository();
        _service = new TransferService(_repository, _itemRepository);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllTransfers()
    {
        _repository.Add(CreateTransfer(1));
        _repository.Add(CreateTransfer(2));

        var result = await _service.GetAllAsync();
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsTransferById()
    {
        _repository.Add(CreateTransfer(1));

        var result = await _service.GetByIdAsync(1);
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Idk", result.Reference);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenTransferDoesNotExist()
    {
        var result = await _service.GetByIdAsync(1);
        Assert.Null(result);
    }

    [Fact]
    public async Task GetItemsInTransferAsync_ReturnsItems()
    {
        var transferItem = CreateTransferItem(1);
        _itemRepository.Add(transferItem);

        var result = await _service.GetItemsInTransferAsync(1);
        Assert.Single(result);
        Assert.Equal(1, result[0].Id);
        Assert.Equal(1, result[0].TransferId);
        Assert.Equal(1, result[0].ItemId);
        Assert.Equal(5, result[0].Amount);
    }

    [Fact]
    public async Task GetItemsInTransferAsync_ReturnsEmptyList_WhenNoItemsExist()
    {
        var result = await _service.GetItemsInTransferAsync(1);
        Assert.Empty(result);
    }

    [Fact]
    public async Task CreateAsync_CreatesTransfer()
    {
        var dto = new CreateTransferDto
        {
            Reference = "Idk",
            ToLocationId = 1,
            FromLocationId = 2,
            TransferStatus = "Scheduled"
        };

        var result = await _service.CreateAsync(dto);

        Assert.Equal("Idk", result.Reference);
        Assert.Equal(1, result.ToLocationId);
        Assert.Equal(2, result.FromLocationId);
        Assert.Single(_repository.Transfers);
        Assert.Equal(1, _repository.SaveChangesCount);
    }

    [Fact]
    public async Task UpdateAsync_UpdatesExistingTransfer()
    {
        var transfer = CreateTransfer(1);
        _repository.Add(transfer);

        var dto = new UpdateTransferDto
        {
            Reference = "Idk",
            ToLocationId = 3,
            FromLocationId = 4,
            TransferStatus = "Scheduled"
        };

        var result = await _service.UpdateAsync(1, dto);

        Assert.NotNull(result);
        Assert.Equal("Idk", result.Reference);
        Assert.Equal(3, result.ToLocationId);
        Assert.Equal(4, result.FromLocationId);
        Assert.Equal(1, _repository.SaveChangesCount);
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenTransferDoesNotExist()
    {
        var dto = new UpdateTransferDto
        {
            Reference = "Idk",
            ToLocationId = 1,
            FromLocationId = 2,
            TransferStatus = "Scheduled"
        };

        var result = await _service.UpdateAsync(1, dto);

        Assert.Null(result);
        Assert.Equal(0, _repository.SaveChangesCount);
    }

    [Fact]
    public async Task DeleteAsync_DeletesExistingTransfer()
    {
        _repository.Add(CreateTransfer(1));

        var result = await _service.DeleteAsync(1);

        Assert.True(result);
        Assert.Empty(_repository.Transfers);
        Assert.Equal(1, _repository.SaveChangesCount);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNull_WhenTransferDoesNotExist()
    {
        var result = await _service.DeleteAsync(1);

        Assert.Null(result);
        Assert.Empty(_repository.Transfers);
        Assert.Equal(0, _repository.SaveChangesCount);
    }

    private static Transfer CreateTransfer(int id)
    {
        return new Transfer
        {
            Id = id,
            Reference = "Idk",
            FromLocationId = 1,
            ToLocationId = 2,
            TransferStatus = "Scheduled",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static TransferItem CreateTransferItem(int id)
    {
        return new TransferItem
        {
            Id = id,
            TransferId = 1,
            ItemId = 1,
            Amount = 5
        };
    }

    private class TestTransferRepository : IRepository<Transfer>
    {
        public List<Transfer> Transfers { get; } = new();
        public int SaveChangesCount { get; private set; }
        public Task<List<Transfer>> GetAllAsync() => Task.FromResult(Transfers.ToList());
        public Task<Transfer?> GetByIdAsync(int id) => Task.FromResult(Transfers.FirstOrDefault(t => t.Id == id));
        public void Add(Transfer transfer)
        {
            if (transfer.Id == 0)
            {
                transfer.Id = Transfers.Count == 0 ? 1 : Transfers.Max(t => t.Id) + 1;
            }
            Transfers.Add(transfer);
        }

        public void Update(Transfer transfer)
        {

        }

        public void Delete(Transfer transfer)
        {
            Transfers.Remove(transfer);
        }

        public Task<int> SaveChangesAsync()
        {
            SaveChangesCount++;
            return Task.FromResult(1);
        }
    }

    private class TestTransferItemRepository : IRepository<TransferItem>
    {
        private List<TransferItem> TransferItems { get; } = new();
        public int SaveChangesCount { get; private set; }
        public Task<List<TransferItem>> GetAllAsync() => Task.FromResult(TransferItems.ToList());
        public Task<TransferItem?> GetByIdAsync(int id) => Task.FromResult(TransferItems.FirstOrDefault(t => t.Id == id));
        public void Add(TransferItem transferItem)
        {
            if (transferItem.Id == 0)
            {
                transferItem.Id = TransferItems.Count == 0 ? 1 : TransferItems.Max(t => t.Id) + 1;
            }
            TransferItems.Add(transferItem);
        }

        public void Update(TransferItem transferItem)
        {

        }

        public void Delete(TransferItem transferItem)
        {
            TransferItems.Remove(transferItem);
        }

        public Task<int> SaveChangesAsync()
        {
            SaveChangesCount++;
            return Task.FromResult(1);
        }
    }
}