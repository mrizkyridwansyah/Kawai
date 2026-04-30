namespace Kawai.Domain.Models;

public class ProductionQualityJudgementModel
{
    public string LineCode { get; set; }
    public long? RequestId { get; set; }
    public long ProductionId { get; set; }
    public long ProdResultID { get; set; }
    public DateTime ScheduleDate { get; set; }
    public string ItemCode { get; set; }
    public string ResultType { get; set; }
}
