namespace CargoHub.Api.Models;

public class ItemGroup : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string ItemGroupcol { get; set; }
}
