using CargoHub.Api.Dtos;

namespace CargoHub.Api.Services;

public interface IShipmentService
{
    Task<List<ShipmentDto>> GetAllAsync();
    Task<ShipmentDto?> GetByIdAsync(int id);
    Task<List<ShipmentItemDto>> GetShipmentItemsInShipmentAsync(int id);
    Task<List<int>> GetOrderIdsInShipmentAsync(int id);
    Task<ShipmentDto> CreateAsync(CreateShipmentDto dto);
    Task<ShipmentDto?> UpdateAsync(int id, UpdateShipmentDto dto);
    Task UpdateShipmentItemsInShipmentAsync(int id, List<ShipmentItemDto> shipmentItems);
    Task<bool?> DeleteAsync(int id);
}