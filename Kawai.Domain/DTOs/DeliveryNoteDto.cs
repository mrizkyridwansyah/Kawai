namespace Kawai.Domain.DTOs;

public class DeliveryNoteDto: DataTableDto
{
    public string DNNumber { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public DateTime DNDate { get; set; }
    public string BCNumber { get; set; }
    public string BCType { get; set; }
    public DateTime BCDate { get; set; }
    public string VehicleNo { get; set; }
    public bool IsComplete { get; set; }
}

public class DeliveryNoteDetailDto : DataTableDto
{
    public string DNNumber { get; set; }
    public string PONumber { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitClsCode { get; set; }
    public string UnitClsName { get; set; }
    public decimal Qty { get; set; }
    public decimal TotalPacking { get; set; }
}
