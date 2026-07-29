using Kawai.Domain.DTOs;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileStockCardRepository
{
    Task<List<StockCardDto>> GetListStockCard(
        string? itemCode);
    Task<List<ItemDto>> GetDDLItemCode(string keyword);
}

public class StockCardDto
{
    public string? Warehouse { get; set; }
    public string? WarehouseName { get; set; }
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }
    public string? Unit { get; set; }

    public int Good { get; set; }
    public int Hold { get; set; }
    public int Total { get; set; }
}