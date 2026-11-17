namespace CargoHub.Api.Dtos;

public record LocationDto 
{
    public int Id { get; set; }
    public int WarehouseId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
public record CreateLocationDto 
{
    public int WarehouseId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}
public record UpdatedLocationDto 
{
    public int WarehouseId { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
}