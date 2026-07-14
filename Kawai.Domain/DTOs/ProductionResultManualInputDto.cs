namespace Kawai.Domain.DTOs;

public class ProductionResultManualInputDto : DataTableDto
{
    public long ProductionId { get; set; }
    public long? ProdResultId { get; set; }
    public DateTime ScheduleDate { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitCls { get; set; }
    public string UnitClsDesc { get; set; }
    public decimal? PlanQty { get; set; }
    public decimal? ResultQty { get; set; }
    public decimal? RemainingQty { get; set; }
    public string LotNo { get; set; }
    public string BarcodeNo { get; set; }
    public decimal? BarcodeQty { get; set; }
    public string RegisterUser { get; set; }
    public DateTime? RegisterDate { get; set; }
    public string StatusWomin { get; set; }
}

public class ProductionResultManualInputDetailDto : DataTableDto
{
    public long? ProdResultId { get; set; }
    public long ProductionId { get; set; }
    public DateTime ScheduleDate { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string LineCode { get; set; }
    public string LineName { get; set; }
    public string ParentItemCode { get; set; }
    public string ParentItemName { get; set; }
    public string MaterialBarcodeNo { get; set; }
    public string MaterialItemCode { get; set; }
    public string MaterialItemName { get; set; }
    public string ChildClassificationPart { get; set; }
    public string ChildClassificationPartDesc { get; set; }
    public decimal? ResultQty { get; set; }
    public decimal BOMQty { get; set; }
    public decimal RequirementQty { get; set; }
    public decimal? ScanQty { get; set; }
    public decimal? RemainingQty { get; set; }
    public string Status { get; set; }
    public string ProductionStatus { get; set; }

}
