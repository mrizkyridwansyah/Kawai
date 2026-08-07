namespace Kawai.Domain.DTOs;

public class ShippingInstructionFilterDto
{
    public string ShippingInstructionNo { get; set; }
    public string PONumber { get; set; }
    public string DDLDescription { get; set; }
}

public class ShippingInstructionDto
{
    public string Supplier { get; set; }
    public string ShippingInstructionNo { get; set; }
    public DateTime ShippingInstructionDate { get; set; }
    public string PONumber { get; set; }
    public int PO_SeqNo { get; set; }
    public string Item_Code { get; set; }
    public string Item_Name { get; set; }
    public string Unit_Cls { get; set; }
    public string Unit_Desc { get; set; }
    public Decimal Qty { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string SerialNo_From { get; set; }
    public string SerialNo_To { get; set; }
 
    
}

public class ShippingInstructionDetailDto : DataTableDto
{
    public long SIDetailID { get; set; }
    public string Supplier { get; set; }
    public string ShippingInstructionNo { get; set; }
    public DateTime ShippingInstructionDate { get; set; }
    public string PONumber { get; set; }
    public int PO_SeqNo { get; set; }
    public string Item_Code { get; set; }
    public string Item_Name { get; set; }
    public string Unit_Cls { get; set; }
    public string Unit_Desc { get; set; }
    public decimal Qty { get; set; }
    public decimal Qty_Stock { get; set; }
    public decimal Qty_Picking { get; set; }
    public DateTime DeliveryDate { get; set; }
    public string SerialNo_From { get; set; }
    public string Serial_No { get; set; }
    public string SerialNo_To { get; set; }
}

public class ShippingInstructionPickingDto : DataTableDto
{ 

    public bool AlreadyPicking { get; set; }
    public string SerialNo { get; set; }
    public DateTime? PickingDate { get; set; }
    public string PickingTime { get; set; }
    public string PickingBy { get; set; }
    
}

public class ShippingInstructionReportDto : DataTableDto
{

    public string CompanyName { get; set; }
    public string CompanyAddress { get; set; }
    public string Phone { get; set; }
    public string TanggalSurat { get; set; }
    public string Kendaraan { get; set; }
    public string NoKendaraan { get; set; }
    public string No { get; set; }
    public string SJNo { get; set; }
    public string CustPONo { get; set; }
    public string BCType { get; set; }
    public string BCNumber { get; set; }
    public string Qty { get; set; }
    public string Delivery { get; set; }
    public string Model { get; set; }
    public string VehicleNo { get; set; }
    public string Transport { get; set; }
    public string SJDate { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string QtyNG { get; set; }
    public string UnitCls { get; set; }
    public string Remarks { get; set; }
    public string DeliveryBy { get; set; }
    public string ApprovedBy { get; set; }
    public string CheckedBy { get; set; }
    public string ReceivedBy { get; set; }
    public string DeliveryByPosition { get; set; }
    public string ApprovedByPosition { get; set; }
    public string CheckedByPosition { get; set; }
    public string ReceivedByPosition { get; set; }


}



