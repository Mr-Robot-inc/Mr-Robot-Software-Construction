using System.ComponentModel.DataAnnotations;
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
    [Range(1, int.MaxValue)]
    public int TransferId { get; init; }
    [Range(1, int.MaxValue)]
    public int ItemId { get; init; }
    [Range(1, int.MaxValue)]
    public int Amount { get; init; }
}

public record UpdateTransferItemDto
{
    [Range(1, int.MaxValue)]
    public int TransferId { get; init; }
    [Range(1, int.MaxValue)]
    public int ItemId { get; init; }
    [Range(1, int.MaxValue)]
    public int Amount { get; init; }
}