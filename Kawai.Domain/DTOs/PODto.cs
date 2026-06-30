namespace Kawai.Domain.DTOs;

public class PODto : DataTableDto
{
    public string PONumber { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public DateTime PODate { get; set; }
    public string WarehouseCode { get; set; }
    public string WarehouseName { get; set; }
}

public class PODetailDto : DataTableDto
{
    public long? ReceiptId { get; set; }
    public long? ReceiptDetailId { get; set; }
    public string PONumber { get; set; }
    public DateTime PODate { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitClsCode { get; set; }
    public string UnitClsName { get; set; }
    public decimal Qty { get; set; }
    public decimal TotalReceiptQty { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal RemainingQty { get; set; }
    public decimal QtyPacking { get; set; }
    public decimal TotalPacking { get; set; }
    public string NoSeri { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
}

public class ClaimDetailDto : DataTableDto
{
    public long? ReceiptId { get; set; }
    public long? ReceiptDetailId { get; set; }
    public string PONumber { get; set; }
    public DateTime PODate { get; set; }
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitClsCode { get; set; }
    public string UnitClsName { get; set; }
    public decimal Qty { get; set; }
    public decimal TotalReceiptQty { get; set; }
    public decimal ReceiptQty { get; set; }
    public decimal RemainingQty { get; set; }
    public decimal QtyPacking { get; set; }
    public decimal TotalPacking { get; set; }
    public string NoSeri { get; set; }
    public DateTime? ProductionDate { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
}


public class LabelBarcodeDetailDto : DataTableDto
{
    public string BarcodeNo { get; set; }
    public string ReceiptNo { get; set; }
    public string FromCompany { get; set; }
    public string ToCompany { get; set; }
    public string PONumber { get; set; }
    public string ShippingLot { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string Qty { get; set; }
    public string DeliveryDate { get; set; }
    public string DNNumber { get; set; }
    public string ShippingLabelNo { get; set; }

    public string BarcodeLabelTitle { get; set; }
    public string BarcodeLabelFrom { get; set; }
    public string BarcodeLabelTo { get; set; }
    public string BarcodeLabelShippingLot { get; set; }
    public string BarcodeLabelDeliveryDate { get; set; }
    public string BarcodeLabelPONumber { get; set; }
    public string BarcodeLabelDNNumber { get; set; }

}
