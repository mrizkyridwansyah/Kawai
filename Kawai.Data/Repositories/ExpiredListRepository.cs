using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class ExpiredListRepository : IExpiredListRepository
{
    private readonly DbExecutor _dbExecutor;

    public ExpiredListRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ExpiredListDto>> GetList(RequestParameter param)
    {
        string sp = "sp_Wms_Expired_GetList";
        return (await _dbExecutor.QueryListAsync<ExpiredListDto>(sp, param.ToQueryObject())).ToList();
    }
}