namespace CargoHub.Api.Models;

public class Transfer : BaseEntity
{
    public string Reference { get; set; }
    public int ToLocationId { get; set; }
    public int FromLocationId { get; set; }
    public string TransferStatus { get; set; }

    public ICollection<TransferItem> TransferItems { get; set; } = new List<TransferItem>();
}
