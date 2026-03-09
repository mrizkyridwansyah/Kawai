using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class TrolleyClsRepository : ITrolleyClsRepository
{
    private readonly DbExecutor _dbExecutor;

    public TrolleyClsRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<TrolleyClsDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_TrolleyCls_List";
        return (await _dbExecutor.QueryListAsync<TrolleyClsDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<TrolleyClsDto> GetData(string trolley_Cls)
    {
        string sp = "sp_Wms_TrolleyCls_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<TrolleyClsDto>(sp, new { Trolley_Cls = trolley_Cls });
    }

    

    

    public async Task Create(TrolleyCls trolley, string userId)
    {
        string sql = @"sp_Wms_TrolleyCls_Create";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            trolley.Description,
            trolley.Trolley_Cls,
            trolley.Qty,
            RegisterBy = userId
        });
    }

    public async Task Update(TrolleyCls trolley, string userId)
    {
        string sql = @"sp_Wms_TrolleyCls_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            trolley.Description,
            trolley.Trolley_Cls,
            trolley.Qty,
            UpdateBy = userId
        });
    }

    public async Task Remove(string trolley_Cls, string userId)
    {
        string sql = "sp_Wms_TrolleyCls_Delete";
        int i = await _dbExecutor.ExecuteAsync(sql, new { Trolley_Cls = trolley_Cls });
    }

    public async Task<Dictionary<string, object>> Capture(string trolley_Cls)
    {
        string sp = "sp_Wms_TrolleyCls_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { Trolley_Cls = trolley_Cls });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

   
}
