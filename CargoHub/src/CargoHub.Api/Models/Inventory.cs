namespace CargoHub.Api.Models;

public class Inventory : BaseEntity
{
	public int ItemId { get; set; }
	public Item? Item { get; set; }
	public int LocationId { get; set; }
	public Location? Location { get; set; }
	public int QuantityOnHand { get; set; }
	public int QuantityOrdered { get; set; }
	public int QuantityAllocated { get; set; }
}
