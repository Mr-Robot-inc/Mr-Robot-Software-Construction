namespace CargoHub.Api.Models;

public class Location : BaseEntity
{
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null;
    public string Code { get; set; }
    public string Name { get; set; }

}
