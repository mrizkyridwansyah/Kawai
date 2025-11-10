namespace Kawai.Domain.DTOs;

// INI DIPAKE DI WEB & MOBILE
public class ReceiptDto: DataTableDto
{
    public long? Id { get; set; }
    public string ReceiptNo { get; set; }
    public DateTime ReceiptDate { get; set; }
    public string DNNumber { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public DateTime DNDate { get; set; }
    public string BCNumber { get; set; }
    public string BCType { get; set; }
    public DateTime BCDate { get; set; }
    public string VehicleNo { get; set; }
    public string Transport { get; set; }
    public string Remarks { get; set; }
    public DateTime? DeliveryDatePOFrom { get; set; }
    public DateTime? DeliveryDatePOUntil { get; set; }
    public string PONumber { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
}

// INI DIPAKE DI WEB & MOBILE
public class ReceiptDetailDto : DataTableDto
{
    public long Id { get; set; }
    public long ReceiptId { get; set; }
    public string PONumber { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitClsCode { get; set; }
    public string UnitClsName { get; set; }
    public double ExpectedQty { get; set; }
    public double TotalPacking { get; set; }
    public double QtyPacking { get; set; }
    public double ReceiptQty { get; set; }
    public string IQCResult { get; set; }
}

// INI DIPAKE DI MOBILE
public class ReceiptDetailBarcodeDto : DataTableDto
{
    public long Id { get; set; }
    public long ReceiptDetailId { get; set; }
    public long ReceiptId { get; set; }
    public string PONumber { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string BarcodeNo { get; set; }
    public string LotNo { get; set; }
    public int SublotNo { get; set; }
    public double Qty { get; set; }
    public bool IsVerified { get; set; }
    public string VerifiedBy { get; set; }
    public DateTime? VerifiedDate { get; set; }
}

public class ReceiptAndonDto : DataTableDto
{
    public string ReceiptNo { get; set; }
    public DateTime ReceiptDate { get; set; }
    public string SupplierName { get; set; }
    public string DNNumber { get; set; }
    public string ItemName { get; set; }
    public string StatusReceipt { get; set; }
}