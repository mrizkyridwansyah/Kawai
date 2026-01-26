using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class TrolleyRepository : ITrolleyRepository
{
    private readonly DbExecutor _dbExecutor;

    public TrolleyRepository (DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<TrolleyDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_Trolley_List";
        return (await _dbExecutor.QueryListAsync<TrolleyDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<TrolleyDto> GetData(string trolleyCode)
    {
        string sp = "sp_Wms_Trolley_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<TrolleyDto>(sp, new { TrolleyCode = trolleyCode });
    }

    public async Task<List<TrolleyDto>> GetDDL(string keyword)
    {
        string sp = "sp_Wms_Trolley_DDL";
        return (await _dbExecutor.QueryListAsync<TrolleyDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

    

    public async Task Create(Trolley trolley, string userId)
    {
        string sql = @"sp_Wms_Trolley_Create";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            trolley.Description,
            trolley.TrolleyCode,
            trolley.Trolley_Cls,
            trolley.IsActive,
            RegisterBy = userId
        });
    }

    public async Task Update(Trolley trolley, string userId)
    {
        string sql = @"sp_Wms_Trolley_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            trolley.Description,
            trolley.TrolleyCode,
            trolley.Trolley_Cls,
            trolley.IsActive,
            UpdateBy = userId
        });
    }

    public async Task Remove(string trolleyCode, string userId)
    {
        string sql = "sp_Wms_Trolley_Delete";
        int i = await _dbExecutor.ExecuteAsync(sql, new { TrolleyCode = trolleyCode });
    }

    public async Task<Dictionary<string, object>> Capture(string trolleyCode)
    {
        string sp = "sp_Wms_Trolley_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { TrolleyCode = trolleyCode });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

   
}
