
using System.Net;
using System.Net.Http.Json;
using CargoHub.Api.Dtos;

public class WarehousesApiTests : IClassFixture<CargohubApiFactory>
{
    private readonly HttpClient _client;

    public WarehousesApiTests(CargohubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ValidWarehouse_Returns201_AndCanBeRetrieved()
    {
        // Arrange
        var dto = new
        {
            Code = "WH001",
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
        var created = await post.Content.ReadFromJsonAsync<WarehouseDto>();

        var get = await _client.GetAsync(
            $"/api/warehouses/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
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
    public async Task Update_ValidWarehouse_Returns200_AndDatabaseIsUpdated()
    {
        // Arrange - First create a warehouse
        var createDto = new
        {
            Code = "WH002",
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

        var post = await _client.PostAsJsonAsync(
            "/api/warehouses", createDto);

        var created = await post.Content.ReadFromJsonAsync<WarehouseDto>();

        // Updated warehouse information
        var updateDto = new
        {
            Code = "WH002",
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

        // Act - Update the warehouse
        var put = await _client.PutAsJsonAsync(
            $"/api/warehouses/{created!.Id}", updateDto);

        // Assert - Check PUT status code
        Assert.Equal(HttpStatusCode.OK, put.StatusCode);

        // Act - Retrieve warehouse again using GET
        var get = await _client.GetAsync(
            $"/api/warehouses/{created.Id}");

        var warehouse = await get.Content.ReadFromJsonAsync<WarehouseDto>();

        // Assert - Check GET status code
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        // Assert - Check that database contains updated information
        Assert.NotNull(warehouse);
        Assert.Equal("WH002", warehouse.Code);
        Assert.Equal("Updated Warehouse", warehouse.Name);
        Assert.Equal("Nieuwe Haven 20", warehouse.Address);
        Assert.Equal("Amsterdam", warehouse.City);
        Assert.Equal("1011AA", warehouse.ZipCode);
        Assert.Equal("Noord-Holland", warehouse.Province);
        Assert.Equal("Netherlands", warehouse.Country);
        Assert.Equal("Frank Herbert", warehouse.ContactName);
        Assert.Equal("123412321", warehouse.ContactPhone);
        Assert.Equal("frank@example.com", warehouse.ContactEmail);
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
        var put = await _client.PutAsJsonAsync(
            "/api/warehouses/99999", dto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingWarehouse_Returns200_AndGetReturns404()
    {
        // Arrange - Create warehouse
        var dto = new
        {
            Code = "WH003",
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
        var created = await post.Content.ReadFromJsonAsync<WarehouseDto>();

        // Act - Delete warehouse
        var delete = await _client.DeleteAsync(
            $"/api/warehouses/{created!.Id}");

        // Assert - Check DELETE status
        Assert.Equal(HttpStatusCode.OK, delete.StatusCode);

        // Act - Try retrieving deleted warehouse
        var get = await _client.GetAsync(
            $"/api/warehouses/{created.Id}");

        // Assert - Warehouse should no longer exist
        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }
}
