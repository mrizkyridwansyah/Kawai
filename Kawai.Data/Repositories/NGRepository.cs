using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class NGRepository : INGRepository
{
    private readonly DbExecutor _dbExecutor;

    public NGRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<NGDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_NG_List";
        return (await _dbExecutor.QueryListAsync<NGDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<NGDto> GetData(string ngCode)
    {
        string sp = "sp_Wms_NG_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<NGDto>(sp, new { NGCode = ngCode });
    }

    public async Task<List<NGDto>> GetDDL(string keyword)
    {
        string sp = "sp_Wms_NG_DDL";
        return (await _dbExecutor.QueryListAsync<NGDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

    

    public async Task Create(NG ng, string userId)
    {
        string sql = @"sp_Wms_NG_Create";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            ng.NGCode,
            ng.Description,
            ng.IsCommon,
            RegisterBy = userId
        });
    }

    public async Task Update(NG ng, string userId)
    {
        string sql = @"sp_Wms_NG_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            ng.Description,
            ng.NGCode,
            ng.IsCommon,
            UpdateBy = userId
        });
    }

    public async Task Remove(string ngCode, string userId)
    {
        string sql = "sp_Wms_NG_Delete";
        int i = await _dbExecutor.ExecuteAsync(sql, new { NGCode = ngCode });
    }

    public async Task<Dictionary<string, object>> Capture(string ngCode)
    {
        string sp = "sp_Wms_NG_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { NGCode = ngCode });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

   
}
