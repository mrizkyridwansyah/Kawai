namespace Kawai.Domain.DTOs;

public class PhysicalInventoryDto : DataTableDto
{
    public string WarehouseCode { get; set; }
    public string ProductCode { get; set; }
    public string MakerItemCode { get; set; }
    public string ProductDesc { get; set; }
    public string UnitCls { get; set; }
    public string Unit { get; set; }

    public string Address { get; set; }
    public decimal PreMonthStock { get; set; }
    public decimal ReceiptTotal { get; set; }
    public decimal SupplyTotal { get; set; }
    public decimal Loss { get; set; }
    public decimal EndOfMonthStock { get; set; }
    public decimal Inventory { get; set; }
    public decimal Inventory2 { get; set; }
    public decimal Differences { get; set; }
    public string Reason { get; set; }
    public string Reason2 { get; set; }
}

public class PhysicalInventoryUpdateDto : DataTableDto
{
    public string WarehouseCode { get; set; }
    public string ProductCode { get; set; }
    public DateTime Period { get; set; }
    public decimal Inventory { get; set; }
    public string Reason { get; set; }
}