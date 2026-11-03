namespace CargoHub.Api.Models;

public class Transfer : IEntity
{
    public int Id { get; set; }
    public string Reference { get; set; }
    public int To_Location_Id { get; set; }
    public int From_Location_Id { get; set; }
    public string Transfer_Status { get; set; }
    public DateTime Created_At { get; set; }
    public DateTime Updated_At { get; set; }
}
