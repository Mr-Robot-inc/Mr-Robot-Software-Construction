using System.Net;
using System.Net.Http.Json;
using CargoHub.Api.Data;
using CargoHub.Api.Dtos;
using Microsoft.Extensions.DependencyInjection;

namespace CargoHub.Tests.Integration;

public class ClientsApiTests : IClassFixture<CargohubApiFactory>
{
    private readonly CargohubApiFactory _factory;
    private readonly HttpClient _client;

    public ClientsApiTests(CargohubApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private static CreateClientDto ValidClient()
    {
        return new CreateClientDto
        {
            Name = "Dune",
            Address = "babastraat",
            City = "babaland",
            ZipCode = "123123VC",
            Province = "ver",
            Country = "Countryroad",
            ContactName = "Bob",
            ContactPhone = "123456789",
            ContactEmail = "bob@gmail.com",
        };
    }

    [Fact]
    public async Task Create_ValidClient_Returns201_AndCanBeRetrieved()
    {
        // Arrange
        var client = ValidClient();

        // Act
        var post = await _client.PostAsJsonAsync("/api/clients", client);
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);

        var created = await post.Content.ReadFromJsonAsync<ClientDto>();
        var get = await _client.GetAsync($"/api/clients/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task GetById_UnknownId_Returns404()
    {
        var response = await _client.GetAsync("/api/clients/999999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_MissingName_Returns400()
    {
        // Arrange
        var dto = new {
            Address = "Frank Herbert",
            City = "Frank Herbert",
            ZipCode = "Frank Herbert",
            Province = "Frank Herbert",
            Country = "Frank Herbert",
            ContactName = "Frank Herbert",
            ContactPhone = "Frank Herbert",
            ContactEmail = "bob@gmail.com",
        };

        var post = await _client.PostAsJsonAsync("/api/clients", dto);

        Assert.Equal(HttpStatusCode.BadRequest, post.StatusCode);
    }

    [Fact]
    public async Task Update_Unknown_Id_Returns404()
    {
        var dto = ValidClient();

        var put = await _client.PutAsJsonAsync("/api/clients/999999", dto);

        Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);
    }

    [Fact]
    public async Task Delete_Existing_Returns204_AndIsRemovedFromDatabase()
    {
        var post = await _client.PostAsJsonAsync("/api/clients", ValidClient());
        var created = await post.Content.ReadFromJsonAsync<ClientDto>();
        Assert.NotNull(created);

        var response = await _client.DeleteAsync($"/api/clients/{created.Id}");

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Assert.Null(await db.Clients.FindAsync(created.Id));
    }

    [Fact]
    public async Task Update_Existing_Returns200_AndDataChanged()
    {
        var post = await _client.PostAsJsonAsync("/api/clients", ValidClient());
        var created = await post.Content.ReadFromJsonAsync<ClientDto>();
        Assert.NotNull(created);
        var update = new UpdateClientDto
        {
            Name = "Frank Herbert",
            Address = "Frank Herbert",
            City = "Frank Herbert",
            ZipCode = "Frank Herbert",
            Province = "Frank Herbert",
            Country = "Frank Herbert",
            ContactName = "Frank Herbert",
            ContactPhone = "Frank Herbert",
            ContactEmail = "FrankHerbert@gmail.com",
        };

        var response = await _client.PutAsJsonAsync($"/api/clients/{created.Id}", update);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var saved = await db.Clients.FindAsync(created.Id);

        Assert.NotNull(saved);
        Assert.Equal("Frank Herbert", saved.Name);
        Assert.Equal("Frank Herbert", saved.Address);
        Assert.Equal("Frank Herbert", saved.City);
        Assert.Equal("Frank Herbert", saved.ZipCode);
        Assert.Equal("Frank Herbert", saved.Province);
        Assert.Equal("Frank Herbert", saved.Country);
        Assert.Equal("Frank Herbert", saved.ContactName);
        Assert.Equal("Frank Herbert", saved.ContactPhone);
        Assert.Equal("Frank Herbert", saved.ContactEmail);

    }

    [Fact]
    public async Task Delete_UnknownId_Returns404()
    {
        var response = await _client.DeleteAsync("/api/clients/999999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidEmail_Returns400()
    {
        var dto = ValidClient() with { ContactEmail = "geen-email" };

        var response = await _client.PostAsJsonAsync("/api/clients", dto);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}