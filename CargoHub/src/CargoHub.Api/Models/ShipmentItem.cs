namespace CargoHub.Api.Models
{
    public class ShipmentItem : IEntity
    {
        public int Id { get; set; }
        public int ShipmentId { get; set; }
        public Shipment Shipment { get; set; } = null!;

        public int ItemId { get; set; }
        public Item Item { get; set; } = null!;
        public int Amount { get; set; }
    }
    
}
