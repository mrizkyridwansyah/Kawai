namespace Kawai.Domain.DTOs;

public class MaterialStorageDto
{
    public string RefNo { get; set; }
    public string WarehouseCode { get; set; }
    public string BarcodeNo { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string LotNo { get; set; }
    public int SublotNo { get; set; }
    public decimal Qty { get; set; }
}

public class MaterialStorageSummaryDto
{
    public string RefNo { get; set; }
    public string WarehouseCode { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string LotNo { get; set; }
    public int TotalItem { get; set; }
    public decimal TotalQty { get; set; }
}