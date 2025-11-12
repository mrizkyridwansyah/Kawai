namespace Kawai.Domain.DTOs;

public class MaterialNGDto
{
    public long? SampleId { get; set; }
    public long? InspectionId { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string BarcodeNo { get; set; }
    public string LotNo { get; set; }
    public int SublotNo { get; set; }
    public double Qty { get; set; }
    public double QtyNG { get; set; }
}
