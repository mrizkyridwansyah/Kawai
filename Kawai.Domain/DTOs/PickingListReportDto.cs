namespace Kawai.Domain.DTOs;

public class PickingListReportDto : DataTableDto
{
    public string Cust_Code { get; set; }
    public string Trade_Name { get; set; }
    public string SI_NO { get; set; }
    public DateTime? SI_Date { get; set; }
    public string Item_Code { get; set; }
    public string Item_Name { get; set; }
    public string Serial_No { get; set; }
    public string Address { get; set; }
    public DateTime? Picking_Date { get; set; }
    public TimeSpan? Picking_Time { get; set; }
    public string Picking_By { get; set; }
    public string Picking_Name { get; set; }
}
