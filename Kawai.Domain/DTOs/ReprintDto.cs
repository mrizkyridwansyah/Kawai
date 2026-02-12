namespace Kawai.Domain.DTOs;

public class ReprintDto: DataTableDto
{
    public string BarcodeNo { get; set; }   
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string Warehouse { get; set; }
    public string Area { get; set; }
    public string Address { get; set; }
    public string LotNo { get; set; }
    public string SubLotNo { get; set; }
    public int? Qty { get; set; }
    public string Source { get; set; }
    public DateTime PrintDate { get; set; }
    public string PrintUser { get; set; }
    
}
