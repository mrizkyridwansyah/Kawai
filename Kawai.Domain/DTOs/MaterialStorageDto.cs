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
    public double Qty { get; set; }
}

public class MaterialStorageSummaryDto
{
    public string WarehouseCode { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string LotNo { get; set; }
    public int TotalItem { get; set; }
    public double TotalQty { get; set; }
}