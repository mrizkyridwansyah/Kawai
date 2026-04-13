using Dapper;
using Kawai.Api.Models;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using System.Data;
using System.Diagnostics;

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
        //ini list StopPoint dari payload
        var stopPoints = wssettinglist.SettingList
            .Where(x => x.AllowSetting == true && !string.IsNullOrEmpty(x.StopPointCode))
            .Select(x => x.StopPointCode )
            .Distinct()
            .ToList();

        var stopPoints2 = wssettinglist.SettingList
           .Where(x => x.AllowSetting == true && !string.IsNullOrEmpty(x.StopPointCode2))
           .Select(x => x.StopPointCode2)
           .Distinct()
           .ToList();

        var stopPoints3 = wssettinglist.SettingList
        .Where(x => x.AllowSetting == true && !string.IsNullOrEmpty(x.StopPointCode3))
        .Select(x => x.StopPointCode3)
        .Distinct()
        .ToList();

        // =========================
        // Build Table Valued Parameter
        // =========================
        var table = new DataTable();
        table.Columns.Add("StopPointCode", typeof(string));
        foreach (var sp in stopPoints)
        {
            table.Rows.Add(sp);
        }

        var table2 = new DataTable();
        table2.Columns.Add("StopPointCode2", typeof(string));
        foreach (var sp2 in stopPoints2)
        {
            table2.Rows.Add(sp2);
        }

        var table3 = new DataTable();
        table3.Columns.Add("StopPointCode3", typeof(string));
        foreach (var sp3 in stopPoints3)
        {
            table3.Rows.Add(sp3);
        }

        var parameters = new DynamicParameters();
        parameters.Add("@StopPoints", table.AsTableValuedParameter("dbo.tvp_StopPointList"));
        parameters.Add("@StopPoints2", table2.AsTableValuedParameter("dbo.tvp_StopPointList2"));
        parameters.Add("@StopPoints3", table3.AsTableValuedParameter("dbo.tvp_StopPointList3"));
        parameters.Add("@LineCode", wssettinglist.LineCode);

        commands.Add((
           "sp_WMS_WorkStationSetting_CheckStopPoints",
           parameters,
           CommandType.StoredProcedure
        ));


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
                wsSet.AllowSetting,
                wsSet.StopPointCode, //input address untuk prod result
                wsSet.StopPointCode2, //input address untuk prod result
                wsSet.StopPointCode3, //input address untuk prod result
                UserID = userId
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
