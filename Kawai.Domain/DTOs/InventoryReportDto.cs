namespace Kawai.Domain.DTOs;

public class InventoryReportDto : DataTableDto
{
    //public string AreaCode { get; set; } //14-07-26 request summary tanpa area & lot
    public string Warehouse { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    //public string LotNo { get; set; } //14-07-26 request summary tanpa area & lot
    public decimal PreMonth { get; set; }
    public decimal Receipt { get; set; }
    public decimal Supply { get; set; }
    public decimal LossReject { get; set; }
    public decimal Current { get; set; }
    public decimal Allocation { get; set; }
    public decimal Ready { get; set; }
    public int Total { get; set; }
    public decimal QtyPalletizer { get; set; }
    public decimal Inventory { get; set; }
    public string Remarks { get; set; }
    public string LastUser { get; set; }
}

public class InventoryListScanReportDto : DataTableDto
{
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string SerialNumber { get; set; }
    public string Status { get; set; }
    public DateTime ScanTime { get; set; }

}
