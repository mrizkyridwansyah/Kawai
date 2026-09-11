namespace Kawai.Domain.Models;

public class PartMaterialRequestWominModel
{
    public string LineCode { get; set; }
    public long? RequestId { get; set; }
    public long ProductionId { get; set; }
    public DateTime ScheduleDate { get; set; }
    public DateTime StartScan { get; set; }
    public string ItemCode { get; set; }
    public decimal RequestSetQty { get; set; }
}

public class PartMaterialRequestWominEditHeaderModel
{
    public long RequestId { get; set; }
    public DateTime StartScan { get; set; }
}

public class PartMaterialRequestWominEditModel
{
    public long IDSeq { get; set; }
    public string ChilItemCode { get; set; }
    public decimal? ReqQty { get; set; }

    
}
