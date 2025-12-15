using Kawai.Api.Models;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using System.Data;

namespace Kawai.Data.Repositories;

public class BOMWorkStationRepository : IBOMWorkStationRepository
{
    private readonly DbExecutor _dbExecutor;

    public BOMWorkStationRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<BOMWorkStationDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_BOMWorkStation_List";
        return (await _dbExecutor.QueryListAsync<BOMWorkStationDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<List<ParentBOMWorkStationDto>> GetBOMWorkStation(string parentitem_code, string workstationcode)
    {
        string sp = "sp_Wms_BOMWorkStation_Detail";
        return (await _dbExecutor.QueryListAsync<ParentBOMWorkStationDto>(sp, new { ParentItem_Code = parentitem_code, WorkStationCode = workstationcode })).ToList();
    }

    public async Task<List<BOMWorkStationDto>> GetModelClsDDL(string keyword)
    {
        string sp = "sp_Wms_BOMWorkStation_ModelClsDDL";
        return (await _dbExecutor.QueryListAsync<BOMWorkStationDto>(sp, new { Keyword = keyword ?? ""})).ToList();
    }

    public async Task<List<BOMWorkStationDto>> GetItemByModelClsDDL(string keyword, string modelCls)
    {
        string sp = "sp_Wms_BOMWorkStation_ItemModelClsDDL";
        return (await _dbExecutor.QueryListAsync<BOMWorkStationDto>(sp, new { Keyword = keyword ?? "", ModelCls = modelCls })).ToList();
    }

    public async Task SaveBOMWorkStation(BOMWorkStation bomsetting, string userId)
    {
        var commands = new List<(string, object?, CommandType)>();

        commands.Add(("sp_WMS_BOMWorkStation_Delete", new
        {
            ParentItem_Code = bomsetting.ParentItem_Code,
            WorkStationCode = bomsetting.WorkStationCode
        }, CommandType.StoredProcedure));

        foreach (var wsSet in bomsetting.BomSetting.Where(p => (p.AllowSetting.HasValue && p.AllowSetting.Value)))
        {
            commands.Add(("sp_WMS_BOMWorkStation_Upd", new
            {
                ParentItem_Code = bomsetting.ParentItem_Code,
                WorkStationCode = bomsetting.WorkStationCode,
                wsSet.ChildItem_Code,
                wsSet.Qty,
                wsSet.AllowSetting,UserID = userId
            }, CommandType.StoredProcedure));
        }

        await _dbExecutor.ExecuteMultiCommandWithTransactionAsync(commands);
    }




    public async Task<Dictionary<string, object>> Capture(string parentitem_code, string workStationCode)
    {
        string spSetting = "sp_Wms_BOMWorkStation_Capture";
        var wsSetting= (await _dbExecutor.QueryListAsync<dynamic>(spSetting, new { ParentItem_Code = parentitem_code, WorkStationCode = workStationCode })).ToList();

         
        return new Dictionary<string, object>
        {
            { "BomSetting", wsSetting } 
        };
    }

   
}
