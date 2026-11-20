namespace CargoHub.Api.Dtos;

public record OrderDto
{
    public int Id { get; init; }
    public int ClientId { get; init; }
    public DateTime OrderDate { get; init; }
    public DateTime RequestDate { get; init; }
    public string? Reference { get; init; }
    public string? CustomerPoNumber { get; init; }
    public string? OrderStatus { get; init; }
    public string? ShippingNotes { get; init; }
    public int WarehouseId { get; init; }
    public int ShipToClientId { get; init; }
    public int BillToClientId { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
    public List<OrderItemDto> Items { get; init; } = [];
}

public record CreateOrderDto
{
    public int ClientId { get; init; }
    public DateTime OrderDate { get; init; }
    public DateTime RequestDate { get; init; }
    public string? Reference { get; init; }
    public string? CustomerPoNumber { get; init; }
    public string? OrderStatus { get; init; }
    public string? ShippingNotes { get; init; }
    public int WarehouseId { get; init; }
    public int ShipToClientId { get; init; }
    public int BillToClientId { get; init; }
}

public record UpdateOrderDto
{
    public int ClientId { get; init; }
    public DateTime OrderDate { get; init; }
    public DateTime RequestDate { get; init; }
    public string? Reference { get; init; }
    public string? CustomerPoNumber { get; init; }
    public string? OrderStatus { get; init; }
    public string? ShippingNotes { get; init; }
    public int WarehouseId { get; init; }
    public int ShipToClientId { get; init; }
    public int BillToClientId { get; init; }
}