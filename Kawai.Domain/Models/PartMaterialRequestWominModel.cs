namespace Kawai.Domain.Models;

public class PartMaterialRequestWominModel
{
    public string LineCode { get; set; }
    public long? RequestId { get; set; }
    public long ProductionId { get; set; }
    public DateTime ScheduleDate { get; set; }
    public string ItemCode { get; set; }
    public double RequestSetQty { get; set; }
}
