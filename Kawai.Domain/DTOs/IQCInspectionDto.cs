namespace Kawai.Domain.DTOs;

public class IQCInspectionDto
{
    public long InspectionId { get; set; }
    public long ReceiptId { get; set; }
    public string ReceiptNo { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public string PONumber { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public DateTime? InspectionDate { get; set; }
    public string InspectionResult { get; set; }
    public long InspectorId { get; set; }
    public string InspectorName { get; set; }
    public string Remarks { get; set; }
    public string Source { get; set; }
    public double TotalQtySample { get; set; }
}

public class IQCSampleDetailBarcodeDto
{
    public long ReceiptDetailBarcodeId { get; set; }
    public long ReceiptDetailId { get; set; }
    public long ReceiptId { get; set; }
    public long? SampleId { get; set; }
    public long? InspectionId { get; set; }
    public string PONumber { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string BarcodeNo { get; set; }
    public string LotNo { get; set; }
    public int SublotNo { get; set; }
    public double Qty { get; set; }
    public double QtySample { get; set; }
}
