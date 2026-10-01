namespace Kawai.Domain.DTOs.RackDashboard;

public class RackDashboardWarehouseDto
{
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string Code 
    { 
        get => string.IsNullOrEmpty(_code) ? WarehouseCode : _code; 
        set { _code = value; if (string.IsNullOrEmpty(WarehouseCode)) WarehouseCode = value; } 
    }
    private string _code = string.Empty;

    public string Name 
    { 
        get => string.IsNullOrEmpty(_name) ? WarehouseName : _name; 
        set { _name = value; if (string.IsNullOrEmpty(WarehouseName)) WarehouseName = value; } 
    }
    private string _name = string.Empty;

    public int RegisteredLocationCount { get; set; }
    public int OccupiedLocationCount { get; set; }
    public int StockCount { get; set; }
}

public class RackDashboardAreaDto
{
    public string WarehouseCode { get; set; } = string.Empty;
    public string AreaCode { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public string Code 
    { 
        get => string.IsNullOrEmpty(_code) ? AreaCode : _code; 
        set { _code = value; if (string.IsNullOrEmpty(AreaCode)) AreaCode = value; } 
    }
    private string _code = string.Empty;

    public string Name 
    { 
        get => string.IsNullOrEmpty(_name) ? AreaName : _name; 
        set { _name = value; if (string.IsNullOrEmpty(AreaName)) AreaName = value; } 
    }
    private string _name = string.Empty;

    public int LocationCount { get; set; }
}

public class RackDashboardLocationDto
{
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string AreaCode { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public string AddressCode { get; set; } = string.Empty;
    public string AddressName { get; set; } = string.Empty;
    public decimal TotalQty { get; set; }
    public int StockRowCount { get; set; }
    public int ItemCount { get; set; }
    public string Status { get; set; } = "empty"; // "empty", "occupied", "unmapped"
    public bool IsRegistered { get; set; }
}

public class RackDashboardStockDetailItemDto
{
    public string BarcodeNo { get; set; } = string.Empty;
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public string LotNo { get; set; } = string.Empty;
    public int SublotNo { get; set; }
    public decimal Qty { get; set; }
    public string Unit { get; set; } = "PCS";
    public string StatusReceipt { get; set; } = "Available";
    public string StatusHoldNG { get; set; } = string.Empty;
    public DateTime? ExpiredDate { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? ReceiptDate { get; set; }
    public string Supplier { get; set; } = string.Empty;
}

public class RackDashboardAddressDetailDto
{
    public string WarehouseCode { get; set; } = string.Empty;
    public string WarehouseName { get; set; } = string.Empty;
    public string AreaCode { get; set; } = string.Empty;
    public string AreaName { get; set; } = string.Empty;
    public string AddressCode { get; set; } = string.Empty;
    public string AddressName { get; set; } = string.Empty;
    public string Status { get; set; } = "empty";
    public decimal TotalQty { get; set; }
    public int StockRowCount { get; set; }
    public int ItemCount { get; set; }
    public bool IsRegistered { get; set; }
    public List<RackDashboardStockDetailItemDto> Stocks { get; set; } = new();
}

public class RackDashboardKpiItemDto
{
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal TotalQty { get; set; }
    public int LotCount { get; set; }
}

public class RackDashboardKpiSummaryDto
{
    public int TotalLocations { get; set; }
    public int EmptyLocations { get; set; }
    public int OccupiedLocations { get; set; }
    public int UnmappedLocations { get; set; }
    public decimal TotalStockQty { get; set; }
    public int TotalStockRows { get; set; }
    public int TotalItemCount { get; set; }
    public double EmptyPercentage { get; set; }
    public List<RackDashboardKpiItemDto> Items { get; set; } = new();
}

