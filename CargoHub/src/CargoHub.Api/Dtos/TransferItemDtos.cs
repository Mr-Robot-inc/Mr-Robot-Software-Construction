namespace CargoHub.Api.Dtos;

public record TransferItemDto
{
    public int Id { get; init; }
    public int TransferId { get; init; }
    public int ItemId { get; init; }
    public int Amount { get; init; }
}

public record CreateTransferItemDto
{
    public int TransferId { get; init; }
    public int ItemId { get; init; }
    public int Amount { get; init; }
}

public record UpdateTransferItemDto
{
    public int TransferId { get; init; }
    public int ItemId { get; init; }
    public int Amount { get; init; }
}