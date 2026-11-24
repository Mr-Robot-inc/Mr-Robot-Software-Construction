using System.ComponentModel.DataAnnotations;

namespace CargoHub.Api.Dtos;

public record ShipmentDto
{
    public int Id { get; init; }
    public string Reference { get; init; } = string.Empty;
    public int OrderId { get; init; }
    public DateTime ShipmentDate { get; init; }
    public string ShipmentType { get; init; } = string.Empty;
    public string ShipmentStatus { get; init; } = string.Empty;
    public string CarrierName { get; init; } = string.Empty;
    public string ShippingMethod { get; init; } = string.Empty;
    public string PaymentType { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CreateShipmentDto
{
    [Required]
    public string Reference { get; init; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int OrderId { get; init; }
    [Required]
    public DateTime ShipmentDate { get; init; }
    [Required]
    public string ShipmentType { get; init; } = string.Empty;
    [Required]
    public string ShipmentStatus { get; init; } = string.Empty;
    [Required]
    public string CarrierName { get; init; } = string.Empty;
    [Required]
    public string ShippingMethod { get; init; } = string.Empty;
    [Required]
    public string PaymentType { get; init; } = string.Empty;
}

public record UpdateShipmentDto
{
    [Required]
    public string Reference { get; init; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int OrderId { get; init; }
    [Required]
    public DateTime ShipmentDate { get; init; }
    [Required]
    public string ShipmentType { get; init; } = string.Empty;
    [Required]
    public string ShipmentStatus { get; init; } = string.Empty;
    [Required]
    public string CarrierName { get; init; } = string.Empty;
    [Required]
    public string ShippingMethod { get; init; } = string.Empty;
    [Required]
    public string PaymentType { get; init; } = string.Empty;
}