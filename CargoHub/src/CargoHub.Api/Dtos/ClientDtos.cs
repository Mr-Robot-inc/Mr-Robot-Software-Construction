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