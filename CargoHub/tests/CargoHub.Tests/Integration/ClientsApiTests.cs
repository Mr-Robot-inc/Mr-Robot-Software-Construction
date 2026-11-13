using System.Net;
using System.Net.Http.Json;
using CargoHub.Api.Dtos;

namespace CargoHub.Tests.Integration;

public class ClientsApiTests : IClassFixture<CargohubApiFactory>
{
    private readonly HttpClient _client;

    public ClientsApiTests(CargohubApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Create_ValidClient_Returns201_AndCanBeRetrieved()
    {
        // Arrange
        var dto = new {
            Name = "Dune",
            Address = "Frank Herbert",
            City = "Frank Herbert",
            ZipCode = "Frank Herbert",
            Province = "Frank Herbert",
            Country = "Frank Herbert",
            ContactName = "Frank Herbert",
            ContactPhone = "Frank Herbert",
            ContactEmail = "Frank Herbert",
        };

        // Act
        var post = await _client.PostAsJsonAsync("/api/clients", dto);
        var created = await post.Content.ReadFromJsonAsync<ClientDto>();
        var get = await _client.GetAsync($"/api/clients/{created!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Created, post.StatusCode);
        Assert.Equal(HttpStatusCode.OK, get.StatusCode);
    }

    [Fact]
    public async Task GetById_UnknownId_Returns404()
    {
        var response = await _client.GetAsync("/api/clients/99999");
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Create_InvalidClient_Returns400()
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
            ContactEmail = "Frank Herbert",
        };

        var post = await _client.PostAsJsonAsync("/api/clients", dto);

        Assert.Equal(HttpStatusCode.BadRequest, post.StatusCode);
    }

    [Fact]
    public async Task Update_Unknown_Id_Returns404()
    {
        var dto = new {
            Name = "Dune",
            Address = "Frank Herbert",
            City = "Frank Herbert",
            ZipCode = "Frank Herbert",
            Province = "Frank Herbert",
            Country = "Frank Herbert",
            ContactName = "Frank Herbert",
            ContactPhone = "Frank Herbert",
            ContactEmail = "Frank Herbert",
        };

        var put = await _client.PutAsJsonAsync("/api/clients/999", dto);

        Assert.Equal(HttpStatusCode.NotFound, put.StatusCode);
    }
}