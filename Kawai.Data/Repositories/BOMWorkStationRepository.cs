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

    public async Task<List< BOMWorkStationDetailDto>> GetBOMWorkStationDetail(string linecode,string parentitem_code, string workstationcode)
    {
        string sp = "sp_Wms_BOMWorkStation_GetDetail";
        return (await _dbExecutor.QueryListAsync<BOMWorkStationDetailDto>(sp, new { LineCode = linecode, ParentItem_Code = parentitem_code, WorkStationCode = workstationcode })).ToList();
    }
    public async Task<List<BOMWorkStationHeaderDto>> GetBOMWorkStationHeader(string linecode,string parentitem_code, string workstationcode)
    {
        string sp = "sp_Wms_BOMWorkStation_GetHeader";
        return (await _dbExecutor.QueryListAsync<BOMWorkStationHeaderDto>(sp, new { LineCode = linecode, ParentItem_Code = parentitem_code, WorkStationCode = workstationcode })).ToList();
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

    public async Task CopyBomWorkStation(string fromline, string toline, string itemcode, string userId)
    {
        string sql = @"sp_Wms_BOMWorkStation_CopyData";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            FromLine = fromline,
            ToLine = toline,
            ItemCode = itemcode,
            UserID = userId
        });
    }


    public async Task SaveBOMWorkStation(BOMWorkStation model, string userId)
    {
      
        var header = model.Header.First();

        var commands = new List<(string, object?, CommandType)>();

        // =========================
        // 1. UPSERT HEADER
        // =========================
        commands.Add((
            "sp_Wms_BOMWorkStation_Header",
            new
            {
                header.FactoryCode,
                header.LineCode,
                header.ModelCls,
                header.ParentItem_Code,
                header.ProcessCode,
                header.QtySet,
                header.Trolley_Cls,
                header.WorkStationCode,
                UserID = userId
            },
            CommandType.StoredProcedure
        ));

        // =========================
        // 2. DELETE DETAIL  
        // =========================
        commands.Add((
            "sp_Wms_BOMWorkStation_DetailDelete",
            new
            {
                header.LineCode,
                header.ParentItem_Code,
                header.WorkStationCode,
               
            },
            CommandType.StoredProcedure
        ));

        // =========================
        // 3. INSERT DETAIL
        // =========================
        foreach (var wsSet in model.Details.Where(p => p.AllowSetting == true))
        {
            commands.Add((
                "sp_WMS_BOMWorkStation_detail_ins",
                new
                {
                    header.LineCode,
                    header.ParentItem_Code,
                    header.WorkStationCode,
                    wsSet.ChildItem_Code,
                    wsSet.Qty,
                    UserID = userId
                },
                CommandType.StoredProcedure
            ));
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
