namespace Kawai.Domain.DTOs;

// INI DIPAKE DI WEB & MOBILE
public class NGClaimDto: DataTableDto
{
    
    public long? ClaimID { get; set; }
    public string ClaimNo { get; set; }
    public DateTime ClaimDate { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public decimal TotalQty { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; }
    public string Notes { get; set; }
    public DateTime DNDate { get; set; }
    public string DNNumber { get; set; }
    public string BCNumber { get; set; }
    public string BCType { get; set; }
    public DateTime BCDate { get; set; }
    public string VehicleNo { get; set; }
    public string Transport { get; set; }
    public string Approved_User { get; set; }
    public DateTime Approved_Date { get; set; }
    public DateTime? ClaimDateFrom { get; set; }
    public DateTime? ClaimDateTo { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
    public DateTime? RegisterDate { get; set; }
    public string RegisterUser { get; set; }
}

// INI DIPAKE DI WEB & MOBILE
public class NGClaimDetailDto : DataTableDto
{
    public long DetailID { get; set; }
    public long ClaimID { get; set; }
    public string PONumber { get; set; }
    public string ReceiptNumber { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitClsCode { get; set; }
    public string UnitClsName { get; set; }
    public decimal Qty { get; set; }
    public string NGCode { get; set; }
    public string NGDescs { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
}

 