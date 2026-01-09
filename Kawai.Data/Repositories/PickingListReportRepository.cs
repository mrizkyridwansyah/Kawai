using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class PickingListReportRepository : IPickingListReportRepository
{
    private readonly DbExecutor _dbExecutor;

    public PickingListReportRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<PickingListReportDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_PickingListReport_GetList";
        return (await _dbExecutor.QueryListAsync<PickingListReportDto>(sp, param.ToQueryObject())).ToList();
    }

}
