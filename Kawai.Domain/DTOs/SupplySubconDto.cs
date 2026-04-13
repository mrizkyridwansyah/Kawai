namespace Kawai.Domain.DTOs;

public class SupplySubconRequestNoDto
{
    public string RequestNo { get; set; }
    public string Description { get; set; }
}

public class SupplySubconDto
{ 
    public string WarehouseCode { get; set; }
    public string BarcodeNo { get; set; }
    public string RequestNo { get; set; }
    public string ProductionDate { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }    
    public string LotNo { get; set; }    
    public decimal PlanQty { get; set; }
    public decimal CurrentQty { get; set; }
    public decimal QtyScan { get; set; }
}