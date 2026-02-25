namespace Kawai.Domain.DTOs;

public class PartMaterialRequestWominDto: DataTableDto
{
    public long ProductionId { get; set; }
    public DateTime ScheduleDate { get; set; }
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

public class PartMaterialRequestWominDetilDto : DataTableDto
{
    public long? RequestId { get; set; }
    public long ProductionId { get; set; }
    public DateTime ScheduleDate { get; set; }
    public string LineCode { get; set; }
    public string LineName { get; set; }
    public string WorkStationCode { get; set; }
    public string WorkStationName { get; set; }
    public string ParentItemCode { get; set; }
    public string ParentItemName { get; set; }
    public string ChildItemCode { get; set; }
    public string ChildItemName { get; set; }
    public string ChildClassificationPart { get; set; }
    public string ChildClassificationPartDesc { get; set; }
    public decimal QtyBOM { get; set; }
    public int SetNumber { get; set; }
    public decimal QtySet { get; set; }
    public decimal RequirementQty { get; set; }
    public string PickingNo { get; set; }
    public string Status { get; set; }
    public decimal? TotalScan { get; set; }
}
