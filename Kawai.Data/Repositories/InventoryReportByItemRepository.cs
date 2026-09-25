using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class InventoryReportByItemRepository : IInventoryReportByItemRepository
{
    private readonly DbExecutor _dbExecutor;

    public InventoryReportByItemRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<InventoryReportByItemDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_InventoryReportByItem_GetList";
        return (await _dbExecutor.QueryListAsync<InventoryReportByItemDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<List<InventoryListReceiptSupplyReportDto>> GetListReceiptSupply(RequestParameter param)
    {
        string sp = "sp_Wms_InventoryReportByItem_GetListReceiptSupply";
        return (await _dbExecutor.QueryListAsync<InventoryListReceiptSupplyReportDto>(sp, param.ToQueryObject())).ToList();
    }

    

}
