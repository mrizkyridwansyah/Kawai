namespace Kawai.Domain.DTOs;

// INI DIPAKE DI WEB & MOBILE
public class ProductionNGClaimDto: DataTableDto
{
    
    public long? ClaimID { get; set; }
    public string ClaimNo { get; set; }
    public DateTime ClaimDate { get; set; }
    public string FactoryCode { get; set; }
    public string FactoryName { get; set; }
    public string ManufactureCode { get; set; }
    public string ManufactureName { get; set; }
    public string LineCode { get; set; }
    public string LineName { get; set; }
    public decimal TotalQty { get; set; }
    public decimal TotalItem { get; set; }
    public string Status { get; set; }
    public string Notes { get; set; }
    public string PickingNo { get; set; }
    public string Priority { get; set; }
    public string Submit_User { get; set; }
    public DateTime Submit_Date { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
    public DateTime? RegisterDate { get; set; }
    public string RegisterUser { get; set; }
}

 
// INI DIPAKE DI WEB & MOBILE
public class ProductionNGClaimDetailDto : DataTableDto
{
    public long DetailID { get; set; }
    public long ClaimID { get; set; }
    public long InspectionID { get; set; }
    public string PickingNo { get; set; }
    public string BarcodeNo { get; set; }
    public string LotNo { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitClsCode { get; set; }
    public string UnitClsName { get; set; }
    public decimal Qty { get; set; }
    public string Reason { get; set; }
    public string RemarksDetail { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
}

 