namespace CargoHub.Api.Dtos;

public record InventoryDto
{
    public int Id { get; init; }
	public int ItemId { get; init; }
	public int LocationId { get; init; }
	public int QuantityOnHand { get; init; }
	public int QuantityOrdered { get; init; }
	public int QuantityAllocated { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime UpdatedAt { get; init; }
}

public record CreateInventoryDto
{
    public int Id { get; init; }
	public int ItemId { get; init; }
	public int LocationId { get; init; }
	public int QuantityOnHand { get; init; }
	public int QuantityOrdered { get; init; }
	public int QuantityAllocated { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime UpdatedAt { get; init; }
}

public record UpdateInventoryDto
{
    public int Id { get; init; }
	public int ItemId { get; init; }
	public int LocationId { get; init; }
	public int QuantityOnHand { get; init; }
	public int QuantityOrdered { get; init; }
	public int QuantityAllocated { get; init; }
	public DateTime CreatedAt { get; init; }
	public DateTime UpdatedAt { get; init; }
}
