namespace Kawai.Domain.DTOs;

public class StockMutationDto
{
    public long Id { get; set; }    
    public string WarehouseCode { get; set; }
    public string AreaCode { get; set; }
    public string AddressCode { get; set; }
    public string ItemCode { get; set; }
    public string BarcodeNo { get; set; }
    public string LotNo { get; set; }
    public decimal QtyTrans { get; set; }
    public int? SublotNo { get; set; }
    public string SourceType { get; set; }
    public string SourceRef { get; set; }
    public string SourceRefNo { get; set; }
    public string RefMutationId { get; set; }
    public string RegisterUser { get; set; }
}
