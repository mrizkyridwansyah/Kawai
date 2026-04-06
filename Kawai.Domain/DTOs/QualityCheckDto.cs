namespace Kawai.Domain.DTOs;

public class QualityCheckDto: DataTableDto
{
    public long InspectionId { get; set; }
    public string Source { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public string DNNumber { get; set; }
    public DateTime DNDate { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitCls { get; set; }
    public string UnitClsDescription { get; set; }
    public decimal Qty { get; set; }
    public decimal QtyNG { get; set; }
    public string InspectionResult { get; set; }
    public DateTime RegisterDate { get; set; }
    public string RegisterUser { get; set; }
    public string RegisterUserName { get; set; }
    public DateTime InspectionDate { get; set; }
    public string InspectorID { get; set; }
    public string InspectorName { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string ApprovalUser { get; set; }
    public string ApprovalUserName { get; set; }
    public string StatusQC { get; set; }

}


public class QualityCheckResultDto: DataTableDto
{
    public long InspectionId { get; set; }
    public string DNNumber { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public decimal Qty { get; set; }
    public decimal QtyNG { get; set; }
    public string InspectionResult { get; set; }
    public string Remarks { get; set; }
    public long? AttachmentID { get; set; }
    public string AttachmentFileName { get; set; }
    public byte[] AttachmentFileBase64 { get; set; }

}

public class QualityCheckReportDto
{
    public string FactoryName { get; set; }
    public string DocumentNo { get; set; }
    public long ReceiptId { get; set; }
    public long InspectionId { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string Model { get; set; }
    public string NoPPR { get; set; }
    public decimal TotalQtyNG { get; set; }
    public string Remarks { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public DateTime InspectionDate { get; set; }
    public DateTime InspectionResultDate { get; set; }
    public string DNNumber { get; set; }
    public DateTime ReceiptDate { get; set; }
}

