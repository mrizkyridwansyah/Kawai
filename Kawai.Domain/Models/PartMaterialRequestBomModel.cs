namespace Kawai.Domain.Models;

public class PartMaterialRequestBomModel
{
    public string WarehouseCode { get; set; }
    public long? RequestId { get; set; }
    public string PONumber { get; set; }
    public DateTime PODate { get; set; }
    public string ItemCode { get; set; }
    public double RequestSetQty { get; set; }
}
