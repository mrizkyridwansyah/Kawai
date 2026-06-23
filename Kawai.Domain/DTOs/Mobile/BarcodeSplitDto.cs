namespace Kawai.Domain.DTOs.Mobile;

public class BarcodeSplitDto
{
    public string BarcodeNo { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string LotNo { get; set; }
    public decimal Qty { get; set; }
}

public class BarcodeSplitHistoryDto
{
    public string BarcodeNo { get; set; }
    public decimal QtySplit { get; set; }
    public string RegisterUser { get; set; }
    public DateTime RegisterDate { get; set; }
}