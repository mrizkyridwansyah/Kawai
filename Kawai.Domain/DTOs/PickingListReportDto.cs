namespace Kawai.Domain.DTOs;

public class PickingListReportDto : DataTableDto
{
    public string AreaCode { get; set; }
    public string Warehouse { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string LotNo { get; set; }
    public decimal PreMonth { get; set; }
    public decimal Receipt { get; set; }
    public decimal Supply { get; set; }
    public decimal LossReject { get; set; }
    public decimal Current { get; set; }
    public decimal Inventory { get; set; }
    public string Remarks { get; set; }
    public string LastUser { get; set; }
}
