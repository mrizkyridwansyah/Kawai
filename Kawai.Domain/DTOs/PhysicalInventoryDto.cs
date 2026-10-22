namespace Kawai.Domain.DTOs;

public class PhysicalInventoryDto : DataTableDto
{
    public string RefNo { get; set; }
    public string WarehouseCode { get; set; }
    public string WarehouseName { get; set; }
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public string AddressCode { get; set; }
    public string AddressName { get; set; }
    public string? BarcodeNo { get; set; }
    public string ItemCode { get; set; }
    public string SerialNo { get; set; }
    public string ItemDesc { get; set; }
    public string UnitCls { get; set; }
    public string Unit { get; set; }
    public string LotNo { get; set; }
    public string StatusScan { get; set; }

    public decimal? CurrentQty { get; set; }
    public decimal? Inventory { get; set; }
    public decimal? Difference { get; set; }

    public DateTime? LastUpdate { get; set; }
    public string? LastUserID { get; set; }
    public string? LastUserName { get; set; }

    public string ParamPeriod { get; set; }
    public string ParamWarehouse { get; set; }
    public string ParamArea { get; set; }
    public string ParamAddress { get; set; }
    public string ParamItem { get; set; }
    public string ParamLot { get; set; }
    public string ParamScanStatus { get; set; }
    public string ParamDiffQty { get; set; }
    public string ParamDiffData { get; set; }


}

public class PhysicalInventoryUpdateDto
{
    public string RefNo { get; set; }
    public string WarehouseCode { get; set; }
    public string AreaCode { get; set; }
    public string AddressCode { get; set; }
    public string BarcodeNo { get; set; }
    public string ItemCode { get; set; }
    public string? LotNo { get; set; }
    public decimal Inventory { get; set; }
}

public class PhysicalInventoryApproveDto
{
    public string Period { get; set; }
  
}

public class PhysicalInventoryCaptureDto
{
    public string RefNo { get; set; }
    public string WarehouseCode { get; set; }
    public string AreaCode { get; set; }
    public string AddressCode { get; set; }
    public string BarcodeNo { get; set; }
    public string ItemCode { get; set; }
    public string LotNo { get; set; }
    public int CurrentQty { get; set; }
    public int? InventoryQty { get; set; }
    public string StatusScan { get; set; }
}

public class PhysicalInventorySummaryStockDto
{
    public string AddressCode { get; set; }
    public string AddressName { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public int TotalBarcode { get; set; }
    public int TotalBarcodeSO { get; set; }
    public decimal CurrentQty { get; set; }
    public decimal? InventoryQty { get; set; }
}