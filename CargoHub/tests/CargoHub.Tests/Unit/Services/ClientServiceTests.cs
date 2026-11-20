using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Services;
using CargoHub.Tests.Unit.Fakes;

namespace CargoHub.Tests.Unit.Services;

public class ClientServiceTests
{
    private readonly FakeRepository<Client> _repo;
    private readonly ClientService _service;


    public ClientServiceTests()
    {
        _repo = new FakeRepository<Client>();
        _service = new ClientService(_repo);
    }

    [Fact]
    public async Task Create_SetsAllFields_AndSaves()
    {
        var dto = new CreateClientDto
        {
            Name = "Mark",
            Address = "babastraat",
            City = "babacity",
            ZipCode = "1423CS",
            Country = "babaland",
            ContactName = "Marcus",
            ContactPhone = "1442312432",
            ContactEmail = "Mark@gmail.com"
        };

        var result = await _service.CreateAsync(dto);

        Assert.Equal("Mark", result.Name);
        Assert.Equal("babastraat", result.Address);
        Assert.Equal("babacity", result.City);
        Assert.Equal("1423CS", result.ZipCode);
        Assert.Equal("babaland", result.Country);
        Assert.Equal("Marcus", result.ContactName);
        Assert.Equal("1442312432", result.ContactPhone);
        Assert.Equal("Mark@gmail.com", result.ContactEmail);
        Assert.True(result.Id > 0);
        Assert.Single(_repo.Items);
        Assert.Equal(1, _repo.SaveCount);
    }

    [Fact]
    public async Task GetById_ExistingId_ReturnsClient()
    {
        var dto = new CreateClientDto
        {
            Name = "Mark",
            Address = "babastraat",
            City = "babacity",
            ZipCode = "1423CS",
            Country = "babaland",
            ContactName = "Marcus",
            ContactPhone = "1442312432",
            ContactEmail = "Mark@gmail.com"
        };

        var result = await _service.CreateAsync(dto);
        var client = await _service.GetByIdAsync(result.Id);

        Assert.NotNull(client);
        Assert.Equal(client.Name, result.Name);
    }

    [Fact]
    public async Task GetById_UnknownId_Null()
    {
        var client = await _service.GetByIdAsync(99999);

        Assert.Null(client);
    }

    [Fact]
    public async Task Update_ExistingId_ChangesFields()
    {
        var created = await _service.CreateAsync(
        new CreateClientDto
        {
            Name = "Mark",
            Address = "babastraat",
            City = "babacity",
            ZipCode = "1423CS",
            Country = "babaland",
            ContactName = "Marcus",
            ContactPhone = "1442312432",
            ContactEmail = "Mark@gmail.com"
        }
        );

        Assert.NotNull(created);

        var update = new UpdateClientDto
        {
            Name = "Bob",
            Address = "abc",
            City = "Polen",
            ZipCode = "1423CS",
            Country = "babaland",
            ContactName = "Marcus",
            ContactPhone = "1442312432",
            ContactEmail = "Mark@gmail.com"
        };

        var result = await _service.UpdateAsync(created.Id, update);

        Assert.NotNull(result);
        Assert.Equal("Bob", result.Name);
        Assert.Equal("abc", result.Address);
        Assert.Equal("Polen", result.City);
        Assert.Equal("1423CS", result.ZipCode);
        Assert.Equal("babaland", result.Country);
        Assert.Equal("Marcus", result.ContactName);
        Assert.Equal("1442312432", result.ContactPhone);
        Assert.Equal("Mark@gmail.com", result.ContactEmail);
        Assert.Equal(2, _repo.SaveCount);
    }

    [Fact]
    public async Task Update_UnknownId_ReturnsNull()
    {
        var update = new UpdateClientDto
        {
            Name = "Bob",
            Address = "abc",
            City = "Polen",
            ZipCode = "1423CS",
            Country = "babaland",
            ContactName = "Marcus",
            ContactPhone = "1442312432",
            ContactEmail = "Mark@gmail.com"
        };

        var result = await _service.UpdateAsync(999, update);

        Assert.Null(result);
        Assert.Equal(0, _repo.SaveCount);
    }

    [Fact]
    public async Task Delete_ExistingId_RemovesClient()
    {

        var created = await _service.CreateAsync( new CreateClientDto
        {
            Name = "Mark",
            Address = "babastraat",
            City = "babacity",
            ZipCode = "1423CS",
            Country = "babaland",
            ContactName = "Marcus",
            ContactPhone = "1442312432",
            ContactEmail = "Mark@gmail.com"
        });

        Assert.NotNull(created);

        var result = await _service.DeleteAsync(created.Id);

        Assert.True(result);
        Assert.Empty(_repo.Items);
    }

    [Fact]
    public async Task Delete_UnknownId_ReturnsFalse()
    {
        var result = await _service.DeleteAsync(999);

        Assert.False(result);
    }
}