namespace CargoHub.Api.Dtos;

public record ItemDto
{
    public int Id { get; set; }
    public string Code  {get; init; }
    public string Description { get; init; }
    public string BarCode { get; init; }
    public string ModeNumber { get; init; }
    public int CommodityCode { get; init; }
    public float UnitWeight { get; init; }
    public int ItemLineId { get; init; }
    public int ItemGroupId { get; init; }
    public int ItemTypeId { get; init; }
    public int MinPurchaseQty { get; init; }
    public int CaseSize { get; init; }
    public string PackagingType { get; init; }
    public int OrderMultiple { get; init; }
    public int SupplierId { get; init; }
    public string SupplierSku { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record CreateItemDto
{
    public int Id { get; set; }
    public string Code  {get; init; }
    public string Description { get; init; }
    public string BarCode { get; init; }
    public string ModeNumber { get; init; }
    public int CommodityCode { get; init; }
    public float UnitWeight { get; init; }
    public int ItemLineId { get; init; }
    public int ItemGroupId { get; init; }
    public int ItemTypeId { get; init; }
    public int MinPurchaseQty { get; init; }
    public int CaseSize { get; init; }
    public string PackagingType { get; init; }
    public int OrderMultiple { get; init; }
    public int SupplierId { get; init; }
    public string SupplierSku { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}

public record UpdateItemDto
{
    public string Code  {get; init; }
    public string Description { get; init; }
    public string BarCode { get; init; }
    public string ModeNumber { get; init; }
    public int CommodityCode { get; init; }
    public float UnitWeight { get; init; }
    public int ItemLineId { get; init; }
    public int ItemGroupId { get; init; }
    public int ItemTypeId { get; init; }
    public int MinPurchaseQty { get; init; }
    public int CaseSize { get; init; }
    public string PackagingType { get; init; }
    public int OrderMultiple { get; init; }
    public int SupplierId { get; init; }
    public string SupplierSku { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; init; }
}
