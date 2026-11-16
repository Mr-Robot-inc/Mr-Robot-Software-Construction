namespace CargoHub.Api.Models;

public class Transfer : IEntity
{
    public int Id { get; set; }
    public string Reference { get; set; }
    public int ToLocationId { get; set; }
    public int FromLocationId { get; set; }
    public string TransferStatus { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
