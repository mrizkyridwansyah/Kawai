using Kawai.Api.Models;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using System.Data;

namespace Kawai.Data.Repositories;

public class WorkStationSettingRepository : IWorkStationSettingRepository
{
    private readonly DbExecutor _dbExecutor;

    public WorkStationSettingRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<WorkStationSettingDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_WorkStationSetting_List";
        return (await _dbExecutor.QueryListAsync<WorkStationSettingDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<List<CompanyLineDto>> GetLineCompanyDDL(string keyword, string companyCode , string manufacture)
    {
        string sp = "sp_Wms_CompanyLine_LineDDL";
        return (await _dbExecutor.QueryListAsync<CompanyLineDto>(sp, new { Keyword = keyword ?? "", CompanyCode = companyCode , Manufacture = manufacture })).ToList();
    }

    public async Task SaveWorkStationSetting(WorkStationSetting wssettinglist, string userId)
    {
        var commands = new List<(string, object?, CommandType)>();

        commands.Add(("sp_WMS_WorkStationSetting_Delete", new
        {
            lineCode = wssettinglist.LineCode
        }, CommandType.StoredProcedure));

        foreach (var wsSet in wssettinglist.SettingList.Where(p => (p.AllowSetting.HasValue && p.AllowSetting.Value)))
        {
            commands.Add(("sp_WMS_WorkStationSetting_Upd", new
            {
                lineCode = wssettinglist.LineCode,
                wsSet.WorkStationCode,
                wsSet.AllowSetting,UserID = userId
            }, CommandType.StoredProcedure));
        }

        await _dbExecutor.ExecuteMultiCommandWithTransactionAsync(commands);
    }




    public async Task<Dictionary<string, object>> Capture(string lineCode)
    {
        string spSetting = "sp_Wms_WorkStationSetting_Capture";
        var wsSetting= (await _dbExecutor.QueryListAsync<dynamic>(spSetting, new { LineCode = lineCode })).ToList();

         
        return new Dictionary<string, object>
        {
            { "WorkStationSetting", wsSetting } 
        };
    }

   
}
