namespace Kawai.Domain.DTOs;

public class ReceiptSupplyHistoryDto: DataTableDto
{
    public string WarehouseCode { get; set; }
    public string ItemCode { get; set; }
    public string LotNo { get; set; }
    public string TransactionType { get; set; }
    public DateTime TransactionDate { get; set; }
    public double QtyTrans { get; set; }
    public string DocReference { get; set; }
    public string Remarks { get; set; }
    public string LastUser { get; set; }
}
