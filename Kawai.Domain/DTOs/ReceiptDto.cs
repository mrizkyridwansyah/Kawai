namespace Kawai.Domain.DTOs;

// INI DIPAKE DI WEB & MOBILE
public class ReceiptDto: DataTableDto
{
    public long? Id { get; set; }
    public string ReceiptNo { get; set; }
    public DateTime ReceiptDate { get; set; }
    public string DNNumber { get; set; }
    public string FactoryCode { get; set; }
    public string FactoryName { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public DateTime DNDate { get; set; }
    public string BCNumber { get; set; }
    public string BCType { get; set; }
    public DateTime BCDate { get; set; }
    public decimal? QtyDN { get; set; }
    public string VehicleNo { get; set; }
    public string Transport { get; set; }
    public string ReferenceNo { get; set; }
    public string RegisterNo { get; set; }
    public string Remarks { get; set; }
    public DateTime? DeliveryDatePOFrom { get; set; }
    public DateTime? DeliveryDatePOUntil { get; set; }
    public string PONumber { get; set; }
    public string StatusReceipt { get; set; }
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
    public decimal ExpectedQty { get; set; }
    public decimal TotalPacking { get; set; }
    public decimal QtyPacking { get; set; }
    public decimal ReceiptQty { get; set; }
    public string IQCResult { get; set; }
    public int? NoSeri { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
}

// INI DIPAKE DI MOBILE
public class ReceiptDetailBarcodeDto : DataTableDto
{
    public long Id { get; set; }
    public long ReceiptDetailId { get; set; }
    public long ReceiptId { get; set; }
    public string DNNumber { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public string PONumber { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string BarcodeNo { get; set; }
    public string LotNo { get; set; }
    public int SublotNo { get; set; }
    public decimal Qty { get; set; }
    public bool IsVerified { get; set; }
    public string VerifiedBy { get; set; }
    public DateTime? VerifiedDate { get; set; }

    // properti ini buat di detail receipt inquiry
    public string WarehouseCode { get; set; }
    public string WarehouseName { get; set; }
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public string AddressCode { get; set; }
    public string AddressName { get; set; }
}

public class ReceiptAndonDto : DataTableDto
{
    public string ReceiptNo { get; set; }
    public DateTime ReceiptDate { get; set; }
    public string SupplierName { get; set; }
    public string DNNumber { get; set; }
    public string ItemName { get; set; }
    public decimal ReceiptQtyUnit { get; set; }
    public string FlagGrid { get; set; }
    public decimal ReceiptQtyPack { get; set; }
    public string StatusReceipt { get; set; }
    public string StatusReceiptName { get; set; }
}

public class ReceiptInquiryDto: DataTableDto
{
    public long? Id { get; set; }
    public long? ReceiptDetailId { get; set; }
    public string ReceiptNo { get; set; }
    public DateTime ReceiptDate { get; set; }
    public string FactoryCode { get; set; }
    public string FactoryName { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string DNNumber { get; set; }
    public DateTime DNDate { get; set; }
    public string PONumber { get; set; }
    public string BCNumber { get; set; }
    public string BCType { get; set; }
    public DateTime BCDate { get; set; }
    public string UnitCls { get; set; }
    public string UnitClsDescription { get; set; }
    public decimal Qty { get; set; }
    public decimal QtyScan { get; set; }
    public string Currency { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
    public string StatusIQC { get; set; }
    public string StatusHoldNG { get; set; }
}

public class ReceiptConfirmationCheckIsDetailsUpdateDto
{
    public bool IsUpdateDetails { get; set; }
    public int TypeConfirmation { get; set; }
    public string TypeConfirmationDesc { get; set; }
}

public class ReceiptBreakdownDto
{
    public long ReceiptEZRId { get; set; }
    public decimal Qty { get; set; }
    public int NoSeri { get; set; }
}