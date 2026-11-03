namespace CargoHub.Api.Models;

public class ShipmentItem : IEntity
{
    public int Id { get; set; }
    public int Shipment_Id { get; set; }
    public int Item_Id { get; set; }
    public int Amount { get; set; }
}
