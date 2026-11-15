using System.Net;
using System.Net.Http.Json;
using CargoHub.Api.Dtos;

namespace CargoHub.Tests.Integration;

public class InvetoriesApiTests : IClassFixture<CargohubApiFactory>
{
    private readonly HttpClient _inventory;

    public InvetoriesApiTests(CargohubApiFactory factory)
    {
        _inventory = factory.CreateClient();
    }

}