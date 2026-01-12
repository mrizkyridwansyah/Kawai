namespace Kawai.Domain.DTOs; 
public class SupplyScanRequestNoDto
{ 
    public string RequestNo{ get; set; } 
    public string Description{ get; set; }
}
public class SupplyScanRequestDto
{ 
    public string WarehouseCode { get; set; }
    public string BarcodeNo { get; set; }
    public string LineCode { get; set; }
    public string RequestNo { get; set; }
    public string ProductionDate { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }    
    public string LotNo { get; set; }    
    public string UnitDesc { get; set; }
    public double PlanQty { get; set; }
    public double QtyScan { get; set; }
}
