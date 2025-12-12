namespace Kawai.Domain.DTOs;

public class StockDto: DataTableDto
{
    public string RefNo { get; set; }
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
    public double InventoryQty { get; set; }
    public DateTime LastUpdate { get; set; }
    public string LastUser { get; set; }
    public string Category { get; set; }
}

public class StockMasterDto: DataTableDto
{
    public string RefNo { get; set; }
    public string WarehouseCode { get; set; }
    public string AreaCode { get; set; }
    public string ItemCode { get; set; }
    public string LotNo { get; set; }
    public double LMPreMonth { get; set; }
    public double LMReceipt { get; set; }
    public double LMSupply { get; set; }
    public double LMCurrent { get; set; }
    public double LMInventory { get; set; }
    public double TMPreMonth { get; set; }
    public double TMReceipt { get; set; }
    public double TMSupply { get; set; }
    public double TMCurrent { get; set; }
    public double TMInventory { get; set; }
    public double NMPreMonth { get; set; }
    public double NMReceipt { get; set; }
    public double NMSupply { get; set; }
    public double NMCurrent { get; set; }
    public double NMInventory { get; set; }
    public string LMReason { get; set; }
    public string TMReason { get; set; }
    public string NMReason { get; set; }
    public double Adjustment { get; set; }
    public List<StockDetailDto> StockDetails { get; set; }
}
public class StockDetailDto : DataTableDto
{
    public string RefNo { get; set; }
    public string WarehouseCode { get; set; }
    public string AreaCode { get; set; }
    public string AddressCode { get; set; }
    public string BarcodeNo { get; set; }
    public string ItemCode { get; set; }
    public string LotNo { get; set; }
    public int SublotNo { get; set; }
    public double Qty { get; set; }
    public double InventoryQty { get; set; }
    public DateTime? ExpiredDate { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public string Supplier { get; set; }
    public bool? PrintCls { get; set; }
    public bool? DisposalCls { get; set; }
    
}