using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class InventoryReportRepository : IInventoryReportRepository
{
    private readonly DbExecutor _dbExecutor;

    public InventoryReportRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<InventoryReportDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_InventoryReport_GetList";
        return (await _dbExecutor.QueryListAsync<InventoryReportDto>(sp, param.ToQueryObject())).ToList();
    }

}
