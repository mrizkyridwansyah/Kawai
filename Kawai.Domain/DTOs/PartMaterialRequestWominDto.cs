namespace Kawai.Domain.DTOs;

public class PartMaterialRequestWominDto: DataTableDto
{
    public long ProductionId { get; set; }
    public DateTime ScheduleDate { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitCls { get; set; }
    public string UnitClsDesc { get; set; }
    public double PlanQty { get; set; }
    public long? RequestId { get; set; }
    public string RequestNo { get; set; }
    public DateTime? RequestDate { get; set; }
    public double? RequestSetQty { get; set; }
    public double? RemainingQty { get; set; }
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
    public double QtyBOM { get; set; }
    public double QtySet { get; set; }
    public double RequirementQty { get; set; }
    public string RegisterUser { get; set; }
    public DateTime? RegisterDate { get; set; }
}
