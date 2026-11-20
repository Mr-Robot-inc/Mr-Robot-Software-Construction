using CargoHub.Api.Dtos;

public interface IShipmentItemService
{
    Task<ShipmentItemDto> CreateAsync(CreateShipmentItemDto dto);
    Task<ShipmentItemDto> GetByIdAsync(int id);
    Task<List<ShipmentItemDto>> GetAllAsync();
    Task<ShipmentItemDto> UpdateAsync(int id, UpdateShipmentItemDto dto);
    Task<bool?> DeleteAsync(int id);
}