namespace Kawai.Domain.DTOs;

public class LoadingTrolleyDto
{
    public long RequestDetailId { get; set; }
    public string RequestDetailNo { get; set; }
    public string RequestDescription { get; set; }
    public string ParentItemCode { get; set; }
    public string ParentItemName { get; set; }
    public string LineCode { get; set; }
    public string LineName { get; set; }
    public string WorkStationCode { get; set; }
    public string WorkStationName { get; set; }
    public string PalletNo { get; set; }
}