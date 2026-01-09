namespace Kawai.Domain.DTOs;

public class PickingListReportDto : DataTableDto
{
    public string AreaCode { get; set; }
    public string Warehouse { get; set; }
    public string ProductCode { get; set; }
    public string ProductName { get; set; }
    public string LotNo { get; set; }
    public double PreMonth { get; set; }
    public double Receipt { get; set; }
    public double Supply { get; set; }
    public double LossReject { get; set; }
    public double Current { get; set; }
    public double Inventory { get; set; }
    public string Remarks { get; set; }
    public string LastUser { get; set; }
}
