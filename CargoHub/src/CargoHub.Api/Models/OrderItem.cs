namespace CargoHub.Api.Models;

public class OrderItem : IEntity
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int ItemId { get; set; }
    public int Amount { get; set; }
    public decimal UnitPrice { get; set; }
}
