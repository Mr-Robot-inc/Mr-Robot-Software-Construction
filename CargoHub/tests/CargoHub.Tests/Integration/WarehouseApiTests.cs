
using System.Net;
using System.Net.Http.Json;
using CargoHub.Api.Dtos;

namespace CargoHub.Tests.Integration;

public class WarehousesApiTests : IClassFixture<CargohubApiFactory>
{
    private readonly HttpClient _client;

    public WarehousesApiTests(CargohubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ValidWarehouse_Returns201_AndDataMatches()
    {
        // Arrange
        var code = $"WH{Guid.NewGuid():N}"[..8];

        var dto = new
        {
            Code = code,
            Name = "Main Warehouse",
            Address = "Nieuwe Haven 20",
            City = "Amsterdam",
            ZipCode = "1011AA",
            Province = "Noord-Holland",
            Country = "Netherlands",
            ContactName = "Frank Herbert",
            ContactPhone = "123456789",
            ContactEmail = "frank@example.com"
        };

        // Act
        var post = await _client.PostAsJsonAsync("/api/warehouses", dto);

        // Assert creation response
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var created = await post.Content.ReadFromJsonAsync<WarehouseDto>();
        Assert.NotNull(created);

        // Check that the created response matches the submitted data
        Assert.Equal(dto.Code, created.Code);
        Assert.Equal(dto.Name, created.Name);
        Assert.Equal(dto.Address, created.Address);
        Assert.Equal(dto.City, created.City);
        Assert.Equal(dto.ZipCode, created.ZipCode);
        Assert.Equal(dto.Province, created.Province);
        Assert.Equal(dto.Country, created.Country);
        Assert.Equal(dto.ContactName, created.ContactName);
        Assert.Equal(dto.ContactPhone, created.ContactPhone);
        Assert.Equal(dto.ContactEmail, created.ContactEmail);

        // Retrieve the warehouse from the API
        var get = await _client.GetAsync($"/api/warehouses/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        var retrieved = await get.Content.ReadFromJsonAsync<WarehouseDto>();
        Assert.NotNull(retrieved);

        // Check that the retrieved data is the same as the created data
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal(dto.Code, retrieved.Code);
        Assert.Equal(dto.Name, retrieved.Name);
        Assert.Equal(dto.Address, retrieved.Address);
        Assert.Equal(dto.City, retrieved.City);
        Assert.Equal(dto.ZipCode, retrieved.ZipCode);
        Assert.Equal(dto.Province, retrieved.Province);
        Assert.Equal(dto.Country, retrieved.Country);
        Assert.Equal(dto.ContactName, retrieved.ContactName);
        Assert.Equal(dto.ContactPhone, retrieved.ContactPhone);
        Assert.Equal(dto.ContactEmail, retrieved.ContactEmail);
    }

    [Fact]
    public async Task GetById_UnknownId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/warehouses/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ValidWarehouse_Returns200_AndDataMatches()
    {
        // Arrange: create a warehouse first
        var code = $"WH{Guid.NewGuid():N}"[..8];

        var createDto = new
        {
            Code = code,
            Name = "Old Warehouse",
            Address = "Old Street 10",
            City = "Rotterdam",
            ZipCode = "3011AA",
            Province = "Zuid-Holland",
            Country = "Netherlands",
            ContactName = "John Doe",
            ContactPhone = "123456789",
            ContactEmail = "john@example.com"
        };

        var post = await _client.PostAsJsonAsync("/api/warehouses", createDto);

        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var created = await post.Content.ReadFromJsonAsync<WarehouseDto>();
        Assert.NotNull(created);

        // New information
        var updateDto = new
        {
            Code = code,
            Name = "Updated Warehouse",
            Address = "Nieuwe Haven 20",
            City = "Amsterdam",
            ZipCode = "1011AA",
            Province = "Noord-Holland",
            Country = "Netherlands",
            ContactName = "Frank Herbert",
            ContactPhone = "123412321",
            ContactEmail = "frank@example.com"
        };

        // Act: update the warehouse
        var put = await _client.PutAsJsonAsync(
            $"/api/warehouses/{created.Id}", updateDto);

        Assert.Equal(HttpStatusCode.OK, put.StatusCode);

        // Retrieve the updated warehouse
        var get = await _client.GetAsync(
            $"/api/warehouses/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        var warehouse = await get.Content.ReadFromJsonAsync<WarehouseDto>();
        Assert.NotNull(warehouse);

        // Assert: compare retrieved data with the update data
        Assert.Equal(created.Id, warehouse.Id);
        Assert.Equal(updateDto.Code, warehouse.Code);
        Assert.Equal(updateDto.Name, warehouse.Name);
        Assert.Equal(updateDto.Address, warehouse.Address);
        Assert.Equal(updateDto.City, warehouse.City);
        Assert.Equal(updateDto.ZipCode, warehouse.ZipCode);
        Assert.Equal(updateDto.Province, warehouse.Province);
        Assert.Equal(updateDto.Country, warehouse.Country);
        Assert.Equal(updateDto.ContactName, warehouse.ContactName);
        Assert.Equal(updateDto.ContactPhone, warehouse.ContactPhone);
        Assert.Equal(updateDto.ContactEmail, warehouse.ContactEmail);
    }

    [Fact]
    public async Task Update_UnknownId_Returns404()
    {
        // Arrange
        var dto = new
        {
            Code = "WH999",
            Name = "Unknown Warehouse",
            Address = "Unknown Street",
            City = "Amsterdam",
            ZipCode = "1011AA",
            Province = "Noord-Holland",
            Country = "Netherlands",
            ContactName = "Frank Herbert",
            ContactPhone = "123456789",
            ContactEmail = "frank@example.com"
        };

        // Act
        var put = await _client.PutAsJsonAsync("/api/warehouses/99999", dto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingWarehouse_Returns200_AndGetReturns404()
    {
        // Arrange
        var code = $"WH{Guid.NewGuid():N}"[..8];

        var dto = new
        {
            Code = code,
            Name = "Warehouse To Delete",
            Address = "Delete Street 1",
            City = "Utrecht",
            ZipCode = "3511AA",
            Province = "Utrecht",
            Country = "Netherlands",
            ContactName = "John Doe",
            ContactPhone = "123456789",
            ContactEmail = "john@example.com"
        };

        var post = await _client.PostAsJsonAsync("/api/warehouses", dto);

        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var created = await post.Content.ReadFromJsonAsync<WarehouseDto>();
        Assert.NotNull(created);

        // Act
        var delete = await _client.DeleteAsync(
            $"/api/warehouses/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, delete.StatusCode);

        var get = await _client.GetAsync(
            $"/api/warehouses/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }
}