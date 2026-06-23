namespace Kawai.Domain.DTOs;

public class PartMaterialRequestBomDto: DataTableDto
{
    public DateTime PODate { get; set; }
    public string PONumber { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitCls { get; set; }
    public string UnitClsDesc { get; set; }
    public decimal PlanQty { get; set; }
    public long? RequestId { get; set; }
    public string RequestNo { get; set; }
    public DateTime? RequestDate { get; set; }
    public decimal? RequestSetQty { get; set; }
    public decimal? RemainingQty { get; set; }
    public string RegisterUser { get; set; }
    public DateTime? RegisterDate { get; set; }
}

public class PartMaterialRequestBomDetilDto : DataTableDto
{
    public long? RequestId { get; set; }
    public DateTime PODate { get; set; }
    public string PONumber { get; set; }
    public string WarehouseCode { get; set; }
    public string WarehouseName { get; set; }
    public string ClassificationCode { get; set; }
    public string ClassificationName { get; set; }
    public string ParentItemCode { get; set; }
    public string ParentItemName { get; set; }
    public string ChildItemCode { get; set; }
    public string ChildItemName { get; set; }
    public decimal QtyBOM { get; set; }
    public decimal QtySet { get; set; }
    public decimal RequirementQty { get; set; }
    public string RegisterUser { get; set; }
    public DateTime? RegisterDate { get; set; }
    public string PickingNo { get; set; }
    public string Status { get; set; }
    public decimal? TotalScan { get; set; }
}

public class PartMaterialRequestBomHeaderDto
{
    public long RequestId { get; set; }
    public string RequestNo { get; set; }
    public string PONo { get; set; }
    public DateTime PODate { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public string SupplierAbbr { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string DNNumber { get; set; }
    public DateTime DNDate { get; set; }
    public string BCNumber { get; set; }
    public string BCType { get; set; }
    public DateTime BCDate { get; set; }
    public string VehicleNo { get; set; }
    public string Transport { get; set; }

}