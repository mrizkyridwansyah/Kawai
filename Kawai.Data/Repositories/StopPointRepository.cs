using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class StopPointRepository : IStopPointRepository
{
    private readonly DbExecutor _dbExecutor;

    public StopPointRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<StopPointDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_StopPoint_List";
        return (await _dbExecutor.QueryListAsync<StopPointDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<StopPointDto> GetData(string stoppointCode)
    {
        string sp = "sp_Wms_StopPoint_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<StopPointDto>(sp, new { StopPointCode = stoppointCode });
    }

    
    

    public async Task Create(StopPoint stoppoint, string userId)
    {
        string sql = @"sp_Wms_StopPoint_Create";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            stoppoint.StopPointCode,
            stoppoint.Description,
            stoppoint.PickingSeq,
            stoppoint.IsActive,
            RegisterBy = userId
        });
    }

    public async Task SaveStopPointAddress(string stoppointCode, string addressKey)
    {
        string sql = @"sp_Wms_StopPoint_AddressSetting";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            StopPointCode = stoppointCode,
            AddressCode = addressKey
        });
    }

    public async Task<List<StopPointDto>> GetDDL(string keyword)
    {
        string sp = "sp_Wms_StopPoint_DDL";
        return (await _dbExecutor.QueryListAsync<StopPointDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }


    public async Task<List<StopPointDto>> GetDDLByAddress(string keyword , string line , string workstation)
    {
        string sp = "sp_Wms_StopPoint_DDLByAddress";
        return (await _dbExecutor.QueryListAsync<StopPointDto>(sp, new { Keyword = keyword ?? "",Line = line  , Workstation = workstation })).ToList();
    }



    public async Task Update(StopPoint stoppoint, string userId)
    {
        string sql = @"sp_Wms_StopPoint_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            stoppoint.Description,
            stoppoint.PickingSeq,
            stoppoint.StopPointCode,
            stoppoint.IsActive,
            UpdateBy = userId
        });
    }

    public async Task Remove(string stoppointCode, string userId)
    {
        string sql = "sp_Wms_StopPoint_Delete";
        int i = await _dbExecutor.ExecuteAsync(sql, new { StopPointCode = stoppointCode });
    }

    public async Task<Dictionary<string, object>> Capture(string stoppointCode)
    {
        string sp = "sp_Wms_StopPoint_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { StopPointCode = stoppointCode });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

   
}
