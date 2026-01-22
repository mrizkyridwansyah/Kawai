namespace Kawai.Domain.DTOs;

public class ReceiptSupplyHistoryDto: DataTableDto
{
    public string ProcessMenu { get; set; }
    public string WarehouseCode { get; set; }
    public string WarehouseName { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string LotNo { get; set; }
    public string TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal PreMonth { get; set; }
    public decimal Receipt { get; set; }
    public decimal Supply { get; set; }
    public decimal Reject { get; set; }
    public decimal Current { get; set; }
    public decimal QtyTrans { get; set; }
    public string FromAreaName { get; set; }
    public string ToAreaName { get; set; }
    public string DocReference { get; set; }
    public string Remarks { get; set; }
    public string LastUser { get; set; }
}
