namespace Kawai.Domain.DTOs;

public class ProductionQualityJudgementDto : DataTableDto
{
    public long ProductionId { get; set; }
    public long? ProdResultId { get; set; }
    public string BarcodeNo { get; set; }
    public DateTime ScheduleDate { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitCls { get; set; }
    public string UnitClsDesc { get; set; }
    public string LotNo { get; set; }
    public bool? Good { get; set; }
    public bool? NG { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
}

public class ProductionQualityJudgementDetailDto : DataTableDto
{
    public long ProductionId { get; set; }
    public long? ProdResultId { get; set; }
    public string BarcodeNo { get; set; }
    public DateTime ScheduleDate { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitCls { get; set; }
    public string UnitClsDesc { get; set; }
    public string LotNo { get; set; }
    public string Good { get; set; }
    public string NG { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }

}
