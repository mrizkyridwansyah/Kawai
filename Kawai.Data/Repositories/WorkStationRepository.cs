using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class WorkStationRepository : IWorkStationRepository
{
    private readonly DbExecutor _dbExecutor;

    public WorkStationRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<WorkStationDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_WorkStation_List";
        return (await _dbExecutor.QueryListAsync<WorkStationDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<WorkStationDto> GetData(string workstationCode)
    {
        string sp = "sp_Wms_WorkStation_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<WorkStationDto>(sp, new { WorkStationCode = workstationCode });
    }

    public async Task<List<WorkStationDto>> GetDDL(string keyword)
    {
        string sp = "sp_Wms_WorkStation_DDL";
        return (await _dbExecutor.QueryListAsync<WorkStationDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

    

    public async Task Create(WorkStation ws, string userId)
    {
        string sql = @"sp_Wms_WorkStation_Create";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            ws.WorkStationCode,
            ws.WorkStationName,
            RegisterBy = userId
        });
    }

    public async Task Update(WorkStation ws, string userId)
    {
        string sql = @"sp_Wms_WorkStation_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            ws.WorkStationCode,
            ws.WorkStationName,
            UpdateBy = userId
        });
    }

    public async Task Remove(string workstationCode, string userId)
    {
        string sql = "sp_Wms_WorkStation_Delete";
        int i = await _dbExecutor.ExecuteAsync(sql, new { WorkStationCode = workstationCode });
    }

    public async Task<Dictionary<string, object>> Capture(string workstationCode)
    {
        string sp = "sp_Wms_WorkStation_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { WorkStationCode = workstationCode });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

   
}
