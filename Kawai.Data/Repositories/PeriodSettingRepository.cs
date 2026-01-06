using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class PeriodSettingRepository : IPeriodSettingRepository
{

    private readonly DbExecutor _dbExecutor;

    public PeriodSettingRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    
    public async Task<List<PeriodSettingDto>> GetListDetail(RequestParameter param)
    {
        var paramYear= param.GetParam("Year");
        
        string sp = "sp_Wms_PeriodSetting_ListDetail";
        return (await _dbExecutor.QueryListAsync<PeriodSettingDto>(sp, new
        {
            Year = paramYear, 
        })).ToList();
    }

   

    public async Task Update(PeriodSetting ps, string userId)
    {
        string sqlHeader = "sp_Wms_PeriodSetting_insupd";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            ps.Year,
            Details = DataTableHelper.ToDataTable(ps.Details),
            UpdateBy = userId
        });
    }

    
    
    public async Task<Dictionary<string, object>> Capture(int year)
    {

        string spSetting = "sp_Wms_PeriodSetting_capture";
        var wsSetting = (await _dbExecutor.QueryListAsync<dynamic>(spSetting, new { Year = year })).ToList();


        return new Dictionary<string, object>
        {
            { "PeriodSetting", wsSetting }
        };

        
    }

 }
