namespace Kawai.Domain.DTOs;

public class PODto : DataTableDto
{
    public string PONumber { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public DateTime PODate { get; set; }
    public string WarehouseCode { get; set; }
    public string WarehouseName { get; set; }
}

public class PODetailDto : DataTableDto
{
    public long? ReceiptId { get; set; }
    public long? ReceiptDetailId { get; set; }
    public string PONumber { get; set; }
    public DateTime PODate { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitClsCode { get; set; }
    public string UnitClsName { get; set; }
    public double Qty { get; set; }
    public double TotalReceiptQty { get; set; }
    public double ReceiptQty { get; set; }
    public double RemainingQty { get; set; }
    public double QtyPacking { get; set; }
    public double TotalPacking { get; set; }
    public string NoSeri { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
}
