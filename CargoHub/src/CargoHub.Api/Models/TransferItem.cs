namespace CargoHub.Api.Models;

public class TransferItem : IEntity
{
    public int Id { get; set; }
    public int Transfer_Id { get; set; }
    public int Item_Id { get; set; }
    public int Amount { get; set; }
}
