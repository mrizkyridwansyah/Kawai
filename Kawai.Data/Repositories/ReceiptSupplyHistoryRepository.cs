using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class ReceiptSupplyHistoryRepository : IReceiptSupplyHistoryRepository
{
    private readonly DbExecutor _dbExecutor;

    public ReceiptSupplyHistoryRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ReceiptSupplyHistoryDto>> GetList(RequestParameter param)
    {
        var filters = param.Filters[0];
        string sp = "sp_Wms_ReceiptSupplyHistory_GetList";
        return (await _dbExecutor.QueryListAsync<ReceiptSupplyHistoryDto>(sp, new
        {
            WarehouseCode = filters["WarehouseCode"],
            ItemCode = filters["ItemCode"],
            LotNo = filters["LotNo"],
            Period = filters["Period"],
        })).ToList();
    }
}
