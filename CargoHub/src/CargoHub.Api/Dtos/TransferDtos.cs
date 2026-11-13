namespace CargoHub.Api.Dtos;

public record TransferDto
{
    public int Id { get; init; }
    public string Reference { get; init; }
    public int ToLocationId { get; init; }
    public int FromLocationId { get; init; }
    public string TransferStatus { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CreateTransferDto
{
    public string Reference { get; init; }
    public int ToLocationId { get; init; }
    public int FromLocationId { get; init; }
    public string TransferStatus { get; init; }
}

public record UpdateTransferDto
{
    public string Reference { get; init; }
    public int ToLocationId { get; init; }
    public int FromLocationId { get; init; }
    public string TransferStatus { get; init; }
}