namespace CargoHub.Api.Models;

public class TransferItem : IEntity
{
    public int Id { get; set; }
    public int TransferId { get; set; }
    public Transfer Transfer { get; set; } = null!;

    public int ItemId { get; set; }
    public int Amount { get; set; }
    public Item Item { get; set; } = null!;
}
