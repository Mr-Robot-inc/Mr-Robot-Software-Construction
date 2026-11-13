
using System.Net;
using System.Net.Http.Json;
using CargoHub.Api.Dtos;

public class LocationsApiTests : IClassFixture<CargohubApiFactory>
{
    private readonly HttpClient _client;

    public LocationsApiTests(CargohubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<int> CreateWarehouse()
    {
        var warehouseDto = new
        {
            Code = $"WH{Guid.NewGuid():N}"[..8],
            Name = "Test Warehouse",
            Address = "Test Street 1",
            City = "Amsterdam",
            ZipCode = "1011AA",
            Province = "Noord-Holland",
            Country = "Netherlands",
            ContactName = "Test User",
            ContactPhone = "123456789",
            ContactEmail = "test@example.com"
        };

        var response = await _client.PostAsJsonAsync(
            "/api/warehouses", warehouseDto);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var warehouse =
            await response.Content.ReadFromJsonAsync<WarehouseDto>();

        Assert.NotNull(warehouse);
        return warehouse.Id;
    }

    [Fact]
    public async Task Create_ValidLocation_Returns201_AndDataMatches()
    {
        // Arrange
        var warehouseId = await CreateWarehouse();

        var dto = new
        {
            WarehouseId = warehouseId,
            Code = "LOC001",
            Name = "Main Location"
        };

        // Act
        var post = await _client.PostAsJsonAsync("/api/locations", dto);

        var created = await post.Content.ReadFromJsonAsync<LocationDto>();

        // Assert
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);
        Assert.NotNull(created);

        Assert.Equal(warehouseId, created.WarehouseId);
        Assert.Equal(dto.Code, created.Code);
        Assert.Equal(dto.Name, created.Name);

        // Retrieve and compare persisted data
        var get = await _client.GetAsync(
            $"/api/locations/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        var retrieved = await get.Content.ReadFromJsonAsync<LocationDto>();

        Assert.NotNull(retrieved);
        Assert.Equal(created.Id, retrieved.Id);
        Assert.Equal(dto.WarehouseId, retrieved.WarehouseId);
        Assert.Equal(dto.Code, retrieved.Code);
        Assert.Equal(dto.Name, retrieved.Name);
    }

    [Fact]
    public async Task GetById_UnknownId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/locations/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ValidLocation_Returns200_AndDataMatches()
    {
        // Arrange
        var warehouseId = await CreateWarehouse();

        var createDto = new
        {
            WarehouseId = warehouseId,
            Code = "LOC002",
            Name = "Old Location"
        };

        var post = await _client.PostAsJsonAsync(
            "/api/locations", createDto);

        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var created = await post.Content.ReadFromJsonAsync<LocationDto>();
        Assert.NotNull(created);

        var updateDto = new
        {
            WarehouseId = warehouseId,
            Code = "LOC002",
            Name = "Updated Location"
        };

        // Act
        var put = await _client.PutAsJsonAsync(
            $"/api/locations/{created.Id}", updateDto);

        // Assert
        Assert.Equal(HttpStatusCode.OK, put.StatusCode);

        // Retrieve updated data
        var get = await _client.GetAsync(
            $"/api/locations/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        var updated = await get.Content.ReadFromJsonAsync<LocationDto>();

        Assert.NotNull(updated);
        Assert.Equal(created.Id, updated.Id);
        Assert.Equal(updateDto.WarehouseId, updated.WarehouseId);
        Assert.Equal(updateDto.Code, updated.Code);
        Assert.Equal(updateDto.Name, updated.Name);
    }

    [Fact]
    public async Task Update_UnknownId_Returns404()
    {
        // Arrange
        var warehouseId = await CreateWarehouse();

        var dto = new
        {
            WarehouseId = warehouseId,
            Code = "LOC999",
            Name = "Unknown Location"
        };

        // Act
        var response = await _client.PutAsJsonAsync(
            "/api/locations/99999", dto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingLocation_Returns200_AndGetReturns404()
    {
        // Arrange
        var warehouseId = await CreateWarehouse();

        var dto = new
        {
            WarehouseId = warehouseId,
            Code = "LOC003",
            Name = "Location To Delete"
        };

        var post = await _client.PostAsJsonAsync(
            "/api/locations", dto);

        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var created = await post.Content.ReadFromJsonAsync<LocationDto>();
        Assert.NotNull(created);

        // Act
        var delete = await _client.DeleteAsync(
            $"/api/locations/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, delete.StatusCode);

        // Confirm that the location no longer exists
        var get = await _client.GetAsync(
            $"/api/locations/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }
}
