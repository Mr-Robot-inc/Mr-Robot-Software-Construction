using System.ComponentModel.DataAnnotations;

namespace CargoHub.Api.Dtos;

public record ClientDto
{
    public int Id { get; init; }
    public string Name { get; init; }
    public string Address { get; init; }
    public string City { get; init; }
    public string ZipCode { get; init; }
    public string Province { get; init; }
    public string Country { get; init; }
    public string ContactName { get; init; }
    public string ContactPhone { get; init; }
    public string ContactEmail { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CreateClientDto
{
    [Required]
    public string Name { get; init; }
    [Required]
    public string Address { get; init; }
    [Required]
    public string City { get; init; }
    [Required]
    public string ZipCode { get; init; }
    [Required]
    public string Province { get; init; }
    [Required]
    public string Country { get; init; }
    [Required]
    public string ContactName { get; init; }
    [Phone]
    [Required]
    public string ContactPhone { get; init; }
    [Required]
    [EmailAddress]
    public string ContactEmail { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record UpdateClientDto
{
    [Required]
    public string Name { get; init; }
    [Required]
    public string Address { get; init; }
    [Required]
    public string City { get; init; }
    [Required]
    public string ZipCode { get; init; }
    [Required]
    public string Province { get; init; }
    [Required]
    public string Country { get; init; }
    [Required]
    public string ContactName { get; init; }
    [Required]
    [Phone]
    public string ContactPhone { get; init; }
    [Required]
    [EmailAddress]
    public string ContactEmail { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
