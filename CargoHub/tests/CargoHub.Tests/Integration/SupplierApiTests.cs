
using System.Net;
using System.Net.Http.Json;
using CargoHub.Api.Dtos;

namespace CargoHub.Tests.Integration;

public class SuppliersApiTests : IClassFixture<CargohubApiFactory>
{
    private readonly HttpClient _client;

    public SuppliersApiTests(CargohubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ValidSupplier_Returns201_AndDataMatches()
    {
        // Arrange
        var code = $"SUP{Guid.NewGuid():N}"[..8];

        var dto = new
        {
            Code = code,
            Name = "Main Supplier",
            Address = "Nieuwe Haven 20",
            City = "Amsterdam",
            ZipCode = "1011AA",
            Province = "Noord-Holland",
            Country = "Netherlands",
            ContactName = "Frank Herbert",
            ContactPhone = "123456789",
            Reference = "SUP-REF-001"
        };

        // Act
        var post = await _client.PostAsJsonAsync("/api/suppliers", dto);

        // Assert creation response
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var created = await post.Content.ReadFromJsonAsync<SupplierDto>();
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
        Assert.Equal(dto.Reference, created.Reference);

        // Retrieve the supplier from the API
        var get = await _client.GetAsync($"/api/suppliers/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        var retrieved = await get.Content.ReadFromJsonAsync<SupplierDto>();
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
        Assert.Equal(dto.Reference, retrieved.Reference);
    }

    [Fact]
    public async Task GetById_UnknownId_Returns404()
    {
        // Act
        var response = await _client.GetAsync("/api/suppliers/99999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Update_ValidSupplier_Returns200_AndDataMatches()
    {
        // Arrange: create a supplier first
        var code = $"SUP{Guid.NewGuid():N}"[..8];

        var createDto = new
        {
            Code = code,
            Name = "Old Supplier",
            Address = "Old Street 10",
            City = "Rotterdam",
            ZipCode = "3011AA",
            Province = "Zuid-Holland",
            Country = "Netherlands",
            ContactName = "John Doe",
            ContactPhone = "123456789",
            Reference = "SUP-OLD-001"
        };

        var post = await _client.PostAsJsonAsync("/api/suppliers", createDto);

        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var created = await post.Content.ReadFromJsonAsync<SupplierDto>();
        Assert.NotNull(created);

        // New information
        var updateDto = new
        {
            Code = code,
            Name = "Updated Supplier",
            Address = "Nieuwe Haven 20",
            City = "Amsterdam",
            ZipCode = "1011AA",
            Province = "Noord-Holland",
            Country = "Netherlands",
            ContactName = "Frank Herbert",
            ContactPhone = "123412321",
            Reference = "SUP-UPDATED-001"
        };

        // Act: update the supplier
        var put = await _client.PutAsJsonAsync(
            $"/api/suppliers/{created.Id}", updateDto);

        Assert.Equal(HttpStatusCode.OK, put.StatusCode);

        // Retrieve the updated supplier
        var get = await _client.GetAsync(
            $"/api/suppliers/{created.Id}");

        Assert.Equal(HttpStatusCode.OK, get.StatusCode);

        var supplier = await get.Content.ReadFromJsonAsync<SupplierDto>();
        Assert.NotNull(supplier);

        // Assert: compare retrieved data with the update data
        Assert.Equal(created.Id, supplier.Id);
        Assert.Equal(updateDto.Code, supplier.Code);
        Assert.Equal(updateDto.Name, supplier.Name);
        Assert.Equal(updateDto.Address, supplier.Address);
        Assert.Equal(updateDto.City, supplier.City);
        Assert.Equal(updateDto.ZipCode, supplier.ZipCode);
        Assert.Equal(updateDto.Province, supplier.Province);
        Assert.Equal(updateDto.Country, supplier.Country);
        Assert.Equal(updateDto.ContactName, supplier.ContactName);
        Assert.Equal(updateDto.ContactPhone, supplier.ContactPhone);
        Assert.Equal(updateDto.Reference, supplier.Reference);
    }

    [Fact]
    public async Task Update_UnknownId_Returns404()
    {
        // Arrange
        var dto = new
        {
            Code = "SUP999",
            Name = "Unknown Supplier",
            Address = "Unknown Street",
            City = "Amsterdam",
            ZipCode = "1011AA",
            Province = "Noord-Holland",
            Country = "Netherlands",
            ContactName = "Frank Herbert",
            ContactPhone = "123456789",
            Reference = "SUP-UNKNOWN-001"
        };

        // Act
        var put = await _client.PutAsJsonAsync("/api/suppliers/99999", dto);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);
    }

    [Fact]
    public async Task Delete_ExistingSupplier_Returns200_AndGetReturns404()
    {
        // Arrange
        var code = $"SUP{Guid.NewGuid():N}"[..8];

        var dto = new
        {
            Code = code,
            Name = "Supplier To Delete",
            Address = "Delete Street 1",
            City = "Utrecht",
            ZipCode = "3511AA",
            Province = "Utrecht",
            Country = "Netherlands",
            ContactName = "John Doe",
            ContactPhone = "123456789",
            Reference = "SUP-DELETE-001"
        };

        var post = await _client.PostAsJsonAsync("/api/suppliers", dto);

        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var created = await post.Content.ReadFromJsonAsync<SupplierDto>();
        Assert.NotNull(created);

        // Act
        var delete = await _client.DeleteAsync(
            $"/api/suppliers/{created.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, delete.StatusCode);

        var get = await _client.GetAsync(
            $"/api/suppliers/{created.Id}");

        Assert.Equal(HttpStatusCode.NotFound, get.StatusCode);
    }
}
