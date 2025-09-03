using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class StockRepository : IStockRepository
{
    private readonly DbExecutor _dbExecutor;

    public StockRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<StockDto>> InquiryByItem(RequestParameter parameter)
    {
        string sp = "sp_Wms_StockInquiry_InquiryByItem";
        return (await _dbExecutor.QueryListAsync<StockDto>(sp, parameter.ToQueryObject())).ToList();
    }
    public async Task<List<StockDto>> InquiryByArea(RequestParameter parameter)
    {
        string sp = "sp_Wms_StockInquiry_InquiryByArea";
        return (await _dbExecutor.QueryListAsync<StockDto>(sp, parameter.ToQueryObject())).ToList();
    }
    public async Task<List<StockDto>> InquiryDetail(RequestParameter parameter)
    {
        string sp = "sp_Wms_StockInquiry_InquiryDetail";
        return (await _dbExecutor.QueryListAsync<StockDto>(sp, parameter.ToQueryObject())).ToList();
    }
    public async Task<List<StockDto>> DDLLotNo(string keyword, string warehouse, string area, string address, string item)
    {
        string sp = "sp_Wms_StockInquiry_DDLLotNo";
        return (await _dbExecutor.QueryListAsync<StockDto>(sp, new
        {
            Keyword = keyword ?? "",
            WarehouseCode = !String.IsNullOrEmpty(warehouse) ? warehouse : "ALL",
            AreaCode = !String.IsNullOrEmpty(area) ? area : "ALL",
            AddressCode = !String.IsNullOrEmpty(address) ? address : "ALL",
            ItemCode = !String.IsNullOrEmpty(item) ? item : "ALL"
        })).ToList();
    }
    public async Task<List<StockDto>> DDLLotNoByStock(string keyword, string warehouse, string area, string address, string item)
    {
        string sp = "sp_Wms_StockInquiry_DDLLotNoByStock";
        return (await _dbExecutor.QueryListAsync<StockDto>(sp, new
        {
            Keyword = keyword ?? "",
            WarehouseCode = !String.IsNullOrEmpty(warehouse) ? warehouse : "ALL",
            AreaCode = !String.IsNullOrEmpty(area) ? area : "ALL",
            AddressCode = !String.IsNullOrEmpty(address) ? address : "ALL",
            ItemCode = !String.IsNullOrEmpty(item) ? item : "ALL"
        })).ToList();
    }
}
