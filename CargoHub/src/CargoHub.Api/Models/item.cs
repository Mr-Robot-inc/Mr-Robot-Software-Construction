public class Item : IEntity 
{
    public int Id { get; set; }
    public string Code { get; set; }
    public string Description { get; set; }
    public string Barcode { get; set; }
    public string Model_number { get; set; }
    public string Community_code { get; set; }
    public decimal Weight { get; set; }
    public int ItemLineId { get; set; }
    public int ItemGroupId { get; set; }
    public int ItemTypeId { get; set; }
    public int MinPurchaseQuantity { get; set; }
    public int CaseSize { get; set; }
    public string PackagingType { get; set; } = "None";
    public int OrderMultiple { get; set; }
    public int SupplierId { get; set; }
    public string SupplierSku { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

}