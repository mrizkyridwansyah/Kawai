namespace Kawai.Domain.DTOs;

public class StockDto: DataTableDto
{
    public string WarehouseCode { get; set; }
    public string WarehouseName { get; set; }
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public string AddressCode { get; set; }
    public string AddressName { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string LotNo { get; set; }
    public string BarcodeNo { get; set; }
    public int SublotNo { get; set; }
    public double PreMonthQty { get; set; }
    public double ReceiptQty { get; set; }
    public double SupplyQty { get; set; }
    public double CurrentQty { get; set; }
    public DateTime LastUpdate { get; set; }
    public string LastUser { get; set; }
}
