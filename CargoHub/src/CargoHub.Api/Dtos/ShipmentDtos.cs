namespace CargoHub.Api.Dtos;

public record ShipmentDto
{
    public int Id { get; init; }
    public string Reference { get; init; }
    public int? OrderId { get; init; }
    public DateTime ShipmentDate { get; init; }
    public string ShipmentType { get; init; }
    public string ShipmentStatus { get; init; }
    public string CarrierName { get; init; }
    public string ShippingMethod { get; init; }
    public string PaymentType { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CreateShipmentDto
{
    public string Reference { get; init; }
    public int OrderId { get; init; }
    public DateTime ShipmentDate { get; init; }
    public string ShipmentType { get; init; }
    public string ShipmentStatus { get; init; }
    public string CarrierName { get; init; }
    public string ShippingMethod { get; init; }
    public string PaymentType { get; init; }
}

public record UpdateShipmentDto
{
    public string Reference { get; init; }
    public int OrderId { get; init; }
    public DateTime ShipmentDate { get; init; }
    public string ShipmentType { get; init; }
    public string ShipmentStatus { get; init; }
    public string CarrierName { get; init; }
    public string ShippingMethod { get; init; }
    public string PaymentType { get; init; }
}