using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IStockRepository
{
    Task<List<StockDto>> InquiryByItem(RequestParameter parameter);
    Task<List<StockDto>> InquiryByArea(RequestParameter parameter);
    Task<List<StockDto>> InquiryByCategory(RequestParameter parameter);
    Task<List<StockDto>> InquiryDetail(RequestParameter parameter);
    Task<List<StockDto>> DDLLotNo(string keyword, string warehouse, string area, string address, string item);
    Task<List<StockDto>> DDLLotNoByStock(string keyword, string warehouse, string area, string address, string item);

    Task RecalculateStockMaster();
    //Task<List<StockDto>> Capture(string keyword, string warehouse, string area, string address, string item);
}
