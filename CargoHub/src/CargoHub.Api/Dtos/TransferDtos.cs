using System.ComponentModel.DataAnnotations;
namespace CargoHub.Api.Dtos;

public record TransferDto
{
    public int Id { get; init; }
    public string Reference { get; init; } = string.Empty;
    public int ToLocationId { get; init; }
    public int FromLocationId { get; init; }
    public string TransferStatus { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CreateTransferDto
{
    [Required]
    public string Reference { get; init; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int ToLocationId { get; init; }
    [Range(1, int.MaxValue)]
    public int FromLocationId { get; init; }
    [Required]
    public string TransferStatus { get; init; } = string.Empty;
}

public record UpdateTransferDto
{
    [Required]
    public string Reference { get; init; } = string.Empty;
    [Range(1, int.MaxValue)]
    public int ToLocationId { get; init; }
    [Range(1, int.MaxValue)]
    public int FromLocationId { get; init; }
    [Required]
    public string TransferStatus { get; init; } = string.Empty;
}