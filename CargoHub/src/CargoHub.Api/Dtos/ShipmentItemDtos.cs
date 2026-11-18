using CargoHub.Api.Models;

namespace CargoHub.Api.Dtos
{
    public record ShipmentItemDtos
    {
        public int Id { get; init; }
        public Shipment Shipment { get; init; }
        public Item Item { get; init; }
        public double Amount { get; init; }
    }
}
