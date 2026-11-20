namespace CargoHub.Api.Models;

public class Order : BaseEntity
{
    public int ClientId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime RequestDate { get; set; }
    public string? Reference { get; set; }
    public string? CustomerPoNumber { get; set; }
    public string? OrderStatus { get; set; }
    public string? ShippingNotes { get; set; }
    public int WarehouseId { get; set; }
    public int ShipToClientId { get; set; }
    public int BillToClientId { get; set; }
}