using CargoHub.Api.Dtos;

namespace CargoHub.Api.Services;

public interface IClientService
{
    Task<List<ClientDto>> GetAllAsync();
    Task<ClientDto?> GetByIdAsync(int id);
    Task<ClientDto> CreateAsync(CreateClientDto dto);
}