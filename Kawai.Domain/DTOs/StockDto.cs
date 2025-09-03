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

public class StockMasterDto: DataTableDto
{
    public string WarehouseCode { get; set; }
    public string AreaCode { get; set; }
    public string ItemCode { get; set; }
    public string LotNo { get; set; }
    public double TMPreMonth { get; set; }
    public double TMReceipt { get; set; }
    public double TMSupply { get; set; }
    public double TMCurrent { get; set; }
    public double TMInventory { get; set; }
    public List<StockDetailDto> StockDetails { get; set; }
}
public class StockDetailDto : DataTableDto
{
    public string WarehouseCode { get; set; }
    public string AreaCode { get; set; }
    public string AddressCode { get; set; }
    public string ItemCode { get; set; }
    public string BarcodeNo { get; set; }
    public string LotNo { get; set; }
    public int SublotNo { get; set; }
    public double Qty { get; set; }
    public double InventoryQty { get; set; }
}
