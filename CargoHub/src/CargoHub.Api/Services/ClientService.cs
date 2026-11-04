using CargoHub.Api.Dtos;
using CargoHub.Api.Models;
using CargoHub.Api.Repositories;
using CargoHub.Api.Services;

namespace Catalog.Api.Services;

public class ClientService : IClientService
{
    private readonly IRepository<Client> _clients;

    public ClientService(IRepository<Client> clients)
    {
        _clients = clients;
    }

    public async Task<List<ClientDto>> GetAllAsync()
    {
        var clients = await _clients.GetAllAsync();
        return clients.Select(ToDto).ToList();
    }

    public async Task<ClientDto?> GetByIdAsync(int id)
    {
        var client = await _clients.GetByIdAsync(id);
        return client is null ? null : ToDto(client);
    }

    public async Task<ClientDto> CreateAsync(CreateClientDto dto)
    {
        var client = new Client
        {
            Name = dto.Name,
            Address = dto.Address,
            City = dto.City,
            ZipCode = dto.ZipCode,
            Province = dto.Province,
            Country = dto.Country,
            ContactName = dto.ContactName,
            ContactPhone = dto.ContactPhone,
            ContactEmail = dto.ContactEmail,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _clients.Add(client);
        await _clients.SaveChangesAsync();

        return ToDto(client);
    }


    private static ClientDto ToDto(Client client) => new()
    {
        Id = client.Id,
        Name = client.Name,
        Address = client.Address,
        City = client.City,
        ZipCode = client.ZipCode,
        Province = client.Province,
        Country = client.Country,
        ContactName = client.ContactName,
        ContactPhone = client.ContactPhone,
        ContactEmail = client.ContactEmail,
        CreatedAt = client.CreatedAt,
        UpdatedAt = client.UpdatedAt

    };
}