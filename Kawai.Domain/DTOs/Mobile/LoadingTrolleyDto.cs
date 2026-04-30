namespace Kawai.Domain.DTOs.Mobile;

public class LoadingTrolleyDto
{
    public string PickingNo { get; set; }
    public string RequestDescription { get; set; }
    public string LineCode { get; set; }
    public string LineName { get; set; }
    public string WorkStationCode { get; set; }
    public string WorkStationName { get; set; }
    public string RefNo { get; set; }
    public string BarcodeNo { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string LotNo { get; set; }
    public decimal Qty { get; set; }
    public string StopPointCode { get; set; }
    public string StopPointName { get; set; }
    public bool StatusScan { get; set; }
}