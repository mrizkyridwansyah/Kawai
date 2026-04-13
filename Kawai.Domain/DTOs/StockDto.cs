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
    public string Status { get; set; }
    public string BarcodeNo { get; set; }
    public int SublotNo { get; set; }
    public decimal PreMonthQty { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal SupplyQty { get; set; }
    public decimal CurrentQty { get; set; }
    public decimal InventoryQty { get; set; }
    public DateTime LastUpdate { get; set; }
    public string LastUser { get; set; }
    public string Category { get; set; }

    /// <summary>
    /// NOT_YET_SO, OK, DIFFERENT
    /// </summary>
    public string StatusSO { get; set; }
}

public class StockMasterDto: DataTableDto
{
    public string RefNo { get; set; }
    public string WarehouseCode { get; set; }
    public string AreaCode { get; set; }
    public string ItemCode { get; set; }
    public string LotNo { get; set; }
    public decimal LMPreMonth { get; set; }
    public decimal LMReceipt { get; set; }
    public decimal LMSupply { get; set; }
    public decimal LMCurrent { get; set; }
    public decimal LMInventory { get; set; }
    public decimal TMPreMonth { get; set; }
    public decimal TMReceipt { get; set; }
    public decimal TMSupply { get; set; }
    public decimal TMCurrent { get; set; }
    public decimal TMInventory { get; set; }
    public decimal NMPreMonth { get; set; }
    public decimal NMReceipt { get; set; }
    public decimal NMSupply { get; set; }
    public decimal NMCurrent { get; set; }
    public decimal NMInventory { get; set; }
    public string LMReason { get; set; }
    public string TMReason { get; set; }
    public string NMReason { get; set; }
    public decimal Adjustment { get; set; }
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
    public decimal Qty { get; set; }
    public decimal InventoryQty { get; set; }
    public DateTime? ExpiredDate { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public string Supplier { get; set; }
    public bool? PrintCls { get; set; }
    public bool? DisposalCls { get; set; }
    
}