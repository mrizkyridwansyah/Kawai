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
    public decimal Qty { get; set; }
    public decimal QtyNG { get; set; }
    public string NGCode { get; set; }
    public string NGDesc { get; set; }
}
