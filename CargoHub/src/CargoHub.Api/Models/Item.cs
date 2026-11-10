namespace CargoHub.Api.Models;

public class Item : IEntity
{
    public int Id { get; set; }
    public string Code  {get; set; }
    public string Description { get; set; }
    public string BarCode { get; set; }
    public string ModeNumber { get; set; }
    public int CommodityCode { get; set; }
    public float UnitWeight { get; set; }
    public int ItemLineId { get; set; }
    public int ItemGroupId { get; set; }
    public int ItemTypeId { get; set; }
    public int MinPurchaseQty { get; set; }
    public int CaseSize { get; set; }
    public string PackagingType { get; set; }
    public int OrderMultiple { get; set; }
    public int SupplierId { get; set; }
    public string SupplierSku { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
