using CargoHub.Api.Models;

namespace CargoHub.Api.Dtos
{
    public record ShipmentItemDto
    {
        public int Id { get; init; }
        public ShipmentShipmentItemDto Shipment { get; init; }
        public ItemShimpentDto Item { get; init; }
        public double Amount { get; init; }
    }
    public record CreateShipmentItemDto
    {
        public int ShipmentId { get; init; }
        public int ItemId { get; init; }
        public double Amount { get; init; }
    }
    public record UpdateShipmentItemDto
    {
        public int ShipmentId { get; init; }
        public int ItemId { get; init; }
        public double Amount { get; init; }
    }
}
