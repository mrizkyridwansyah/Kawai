using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Kawai.Data.Repositories;

public class PartMaterialRequestWominRepository : IPartMaterialRequestWominRepository
{

    private readonly DbExecutor _dbExecutor;

    public PartMaterialRequestWominRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<PartMaterialRequestWominDto>> GetListHeader(RequestParameter param)
    {
        var paramPeriodFrom = param.GetParam("PeriodFrom");
        var paramPeriodUntil = param.GetParam("PeriodUntil");
        var paramFactory = param.GetParam("FactoryCode");
        var paramProcess = param.GetParam("ManufactureCode");
        var paramLine = param.GetParam("LineCode");
        var paramRemainingFuckingCls = param.GetParam("RemainingCls");

        string sp = "sp_Wms_PartMaterialRequestWomin_GetListHeader";
        return (await _dbExecutor.QueryListAsync<PartMaterialRequestWominDto>(sp, new
        {
            PeriodFrom = paramPeriodFrom,
            PeriodUntil = paramPeriodUntil,
            FactoryCode = paramFactory,
            ProcessCode = paramProcess,
            LineCode = paramLine,
            RemainingCls = paramRemainingFuckingCls == "ALL" ? (bool?)null : paramRemainingFuckingCls == "YES"
        })).ToList();
    }

    public async Task<List<PartMaterialRequestWominDetilDto>> GetListDetail(List<PartMaterialRequestWominModel> models)
    {
        string sp = "sp_Wms_PartMaterialRequestWomin_GetListDetail";
        return (await _dbExecutor.QueryListAsync<PartMaterialRequestWominDetilDto>(sp, new
        {
            models[0].LineCode,
            models[0].RequestId,
            models[0].ProductionId,
            models[0].ScheduleDate,
            models[0].ItemCode,
            models[0].RequestSetQty
        })).ToList();
    }
    public async Task<List<StockDto>> GetListStock(RequestParameter param)
    {
        string sp = "sp_Wms_PartMaterialRequest_GetListStock";
        return (await _dbExecutor.QueryListAsync<StockDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<List<PartMaterialRequestWominReportDto>> WominReport(long requestId)
    {
        string sp = "sp_Wms_Andon_WominRequest_Report";
        return (await _dbExecutor.QueryListAsync<PartMaterialRequestWominReportDto>(sp, new { RequestID = requestId })).ToList();
    }


    public async Task<List<StockScanDto>> GetListScan(RequestParameter param)
    {
        string sp = "sp_Wms_PartMaterialRequest_GetListScan";
        return (await _dbExecutor.QueryListAsync<StockScanDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<PartMaterialRequestWominEditDto> GetData(long idSeq)
    {
        string sp = "sp_Wms_PartMaterialRequestWomin_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<PartMaterialRequestWominEditDto>(sp, new { IDSeq = idSeq });
    }

    public async Task UpdateReq(PartMaterialRequestWominEditModel model, string userId)
    {
        string sql = @"sp_Wms_PartMaterialRequestWomin_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            model.IDSeq,
            model.ChilItemCode,
            model.ReqQty,
            UpdateBy = userId
        });
    }

    public async Task<Dictionary<string, object>> CaptureRequirement(long idSeq)
    {
        string sp = "sp_Wms_PartMaterialRequestWomin_CaptureRequirement";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { IDSeq = idSeq });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task Save(List<PartMaterialRequestWominModel> models, string userId)
    {
        string sp = "sp_Wms_PartMaterialRequestWomin_Save";
        await _dbExecutor.ExecuteNonTransactionAsync(sp, new
        {
            models[0].LineCode,
            NewRequest = DataTableHelper.ToDataTable(models, ["LineCode"]),
            UserId = userId
        });
    }

    public async Task Remove(long requestId, string userId)
    {
        string sp = "sp_Wms_PartMaterialRequestWomin_Delete";
        await _dbExecutor.ExecuteAsync(sp, new
        {
            RequestId = requestId,
            UserId = userId
        });
    }

    public async Task<Dictionary<string, object>> Capture(long productionId)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_PartMaterialRequestWomin_Capture",
            param: new { ProductionId = productionId },
            async multi =>
            {
                var headers = (await multi.ReadAsync<dynamic>()).ToList();
                var details = (await multi.ReadAsync<dynamic>()).ToList();
                var detailItems = (await multi.ReadAsync<dynamic>()).ToList();

                foreach (var header in headers)
                {
                    header.Details = details.Where(p => p.RequestID == header.RequestID).ToList();
                    header.DetailItems = detailItems.Where(p => p.RequestID == header.RequestID).ToList();
                }

                return headers;
            }
        );

        return new Dictionary<string, object>
        {
            { "Part Material Request Womin", result },
        };
    }

    public async Task<Dictionary<string, object>> CaptureRequest(long requestId)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_PartMaterialRequestWomin_CaptureRequest",
            param: new { RequestId = requestId },
            async multi =>
            {
                var header = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var details = (await multi.ReadAsync<dynamic>()).ToList();
                var detailItems = (await multi.ReadAsync<dynamic>()).ToList();

                return (header, details, detailItems);
            }
        );

        return new Dictionary<string, object>
        {
            { "Part Material Request Womin Header", result.header },
            { "Part Material Request Womin Details", result.details },
            { "Part Material Request Womin Detail Items", result.detailItems },
        };
    }
}
