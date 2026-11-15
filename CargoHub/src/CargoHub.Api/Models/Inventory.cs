namespace CargoHub.Api.Models;

public class Inventory : IEntity
{
	public int Id { get; set; }
	public int ItemId { get; set; }
	public int LocationId { get; set; }
	public int QuantityOnHand { get; set; }
	public int QuantityOrdered { get; set; }
	public int QuantityAllocated { get; set; }
	public DateTime CreatedAt { get; set; }
	public DateTime UpdatedAt { get; set; }
}
