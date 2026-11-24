namespace CargoHub.Api.Dtos;

public record ShipmentItemDto
{
    public int Id { get; init; }
    public int ShipmentId { get; init; }
    public int ItemId { get; init; }
    public int Amount { get; init; }
}