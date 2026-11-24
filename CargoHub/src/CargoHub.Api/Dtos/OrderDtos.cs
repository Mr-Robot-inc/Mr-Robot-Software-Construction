using System.ComponentModel.DataAnnotations;
namespace CargoHub.Api.Dtos;

public record OrderDto
{
    public int Id { get; init; }
    public int ClientId { get; init; }
    public DateTime OrderDate { get; init; }
    public DateTime RequestDate { get; init; }
    public string Reference { get; init; } = string.Empty;
    public string CustomerPoNumber { get; init; } = string.Empty;
    public string OrderStatus { get; init; } = string.Empty;
    public string ShippingNotes { get; init; } = string.Empty;
    public int WarehouseId { get; init; }
    public int ShipToClientId { get; init; }
    public int BillToClientId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<OrderItemDto> Items { get; init; } = [];
}

public record CreateOrderDto
{
    [Range(1, int.MaxValue)]
    public int ClientId { get; init; }
    [Required]
    public DateTime OrderDate { get; init; }
    [Required]
    public DateTime RequestDate { get; init; }
    [Required]
    public string Reference { get; init; } = string.Empty;
    [Required]
    public string CustomerPoNumber { get; init; } = string.Empty;
    [Required]
    public string OrderStatus { get; init; } = string.Empty;
    [Required]
    public string ShippingNotes { get; init; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int WarehouseId { get; init; }
    [Range(1, int.MaxValue)]
    public int ShipToClientId { get; init; }
    [Range(1, int.MaxValue)]
    public int BillToClientId { get; init; }
    public List<OrderItemDto>? Items { get; init; }
}

public record UpdateOrderDto
{
    [Range(1, int.MaxValue)]
    public int ClientId { get; init; }
    [Required]
    public DateTime OrderDate { get; init; }
    [Required]
    public DateTime RequestDate { get; init; }
    [Required]
    public string Reference { get; init; } = string.Empty;
    [Required]
    public string CustomerPoNumber { get; init; } = string.Empty;
    [Required]
    public string OrderStatus { get; init; } = string.Empty;
    [Required]
    public string ShippingNotes { get; init; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int WarehouseId { get; init; }
    [Range(1, int.MaxValue)]
    public int ShipToClientId { get; init; }
    [Range(1, int.MaxValue)]
    public int BillToClientId { get; init; }
    public List<OrderItemDto>? Items { get; init; }
}