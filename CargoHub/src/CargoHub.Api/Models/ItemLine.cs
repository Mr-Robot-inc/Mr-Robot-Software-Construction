namespace CargoHub.Api.Models;

public class ItemLine : BaseEntity
{
    public string Name { get; set; }
    public string? Description { get; set; }

    // used for foreign keys
    public ICollection<Item> Items { get; set; } = new List<Item>();
}
