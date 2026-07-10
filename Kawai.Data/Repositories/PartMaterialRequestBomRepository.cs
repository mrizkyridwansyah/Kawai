using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class PartMaterialRequestBomRepository : IPartMaterialRequestBomRepository
{

    private readonly DbExecutor _dbExecutor;

    public PartMaterialRequestBomRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<PartMaterialRequestBomDto>> GetListHeader(RequestParameter param)
    {
        var paramPeriodFrom = param.GetParam("PeriodFrom");
        var paramPeriodUntil = param.GetParam("PeriodUntil");
        var paramFactory = param.GetParam("FactoryCode");
        var paramSupplier = param.GetParam("SupplierCode");
        var paramPONumber = param.GetParam("PONumber");
        var paramRemainingCls = param.GetParam("RemainingCls");

        string sp = "sp_Wms_PartMaterialRequestBom_GetListHeader";
        return (await _dbExecutor.QueryListAsync<PartMaterialRequestBomDto>(sp, new
        {
            PeriodFrom = paramPeriodFrom,
            PeriodUntil = paramPeriodUntil,
            FactoryCode = paramFactory,
            SupplierCode = paramSupplier,
            PONumber = paramPONumber,
            RemainingCls = paramRemainingCls == "ALL" ? (bool?)null : paramRemainingCls == "YES"
        })).ToList();
    }

    public async Task<List<PartMaterialRequestBomDetilDto>> GetListDetail(List<PartMaterialRequestBomModel> models)
    {
        string sp = "sp_Wms_PartMaterialRequestBom_GetListDetail";
        return (await _dbExecutor.QueryListAsync<PartMaterialRequestBomDetilDto>(sp, new
        {
            models[0].WarehouseCode,
            models[0].RequestId,
            models[0].PONumber,
            models[0].PODate,
            models[0].ItemCode,
            models[0].RequestSetQty
        })).ToList();
    }
    public async Task<List<StockDto>> GetListStock(RequestParameter param)
    {
        string sp = "sp_Wms_PartMaterialRequest_GetListStock";
        return (await _dbExecutor.QueryListAsync<StockDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<List<NGClaimReportDto>> GetListReport(string requestno)
    {
        string sp = "sp_Wms_PartMaterialRequest_Report";

        return (await _dbExecutor.QueryListAsync<NGClaimReportDto>(sp, new
        {
            RequestNo = requestno 
        })).ToList();
    }

    public async Task<PartMaterialRequestBomHeaderDto> GetDataHeader(long requestId, string itemCode)
    {
        string sp = "sp_Wms_PartMaterialRequestBom_GetDataHeader";
        return await _dbExecutor.QueryFirstOrDefaultAsync<PartMaterialRequestBomHeaderDto>(sp, new { RequestId = requestId, ItemCode = itemCode });
    }

    public async Task Update(PartMaterialRequestBomHeaderModel model, string userId)
    {
        string sp = "sp_Wms_PartMaterialRequestBom_Update";
        await _dbExecutor.ExecuteNonTransactionAsync(sp, new
        {
            model.RequestId,
            model.DNNumber,
            model.DNDate,
            model.BCNumber,
            model.BCType,
            model.BCDate,
            model.VehicleNo,
            model.Transport,
            UserId = userId
        });
    }

    public async Task Save(PartMaterialRequestBomHeaderModel model, string userId)
    {
        string sp = "sp_Wms_PartMaterialRequestBom_Save";
        await _dbExecutor.ExecuteNonTransactionAsync(sp, new
        {
            model.Details[0].WarehouseCode,
            model.DNNumber,
            model.DNDate,
            model.BCNumber,
            model.BCType,
            model.BCDate,
            model.VehicleNo,
            model.Transport,
            NewRequest = DataTableHelper.ToDataTable(model.Details, ["WarehouseCode"]),
            UserId = userId
        });
    }

    public async Task Remove(long requestId, string userId)
    {
        string sp = "sp_Wms_PartMaterialRequestBom_Delete";
        await _dbExecutor.ExecuteAsync(sp, new
        {
            RequestId = requestId,
            UserId = userId
        });
    }

    public async Task<Dictionary<string, object>> Capture(string poNumber)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_PartMaterialRequestBom_Capture",
            param: new { PONumber = poNumber },
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
            { "Part Material Request Bom", result },
        };
    }

    public async Task<Dictionary<string, object>> CaptureRequest(long requestId)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_PartMaterialRequestBom_CaptureRequest",
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
            { "Part Material Request Bom Header", result.header },
            { "Part Material Request Bom Details", result.details },
            { "Part Material Request Bom Detail Items", result.detailItems },
        };
    }

    public async Task<List<StockScanDto>> GetListScan(RequestParameter param)
    {
        string sp = "sp_Wms_PartMaterialRequestBom_GetListScan";
        return (await _dbExecutor.QueryListAsync<StockScanDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<PartMaterialRequestBomDetilItemDto> GetData(long idSeq)
    {
        string sp = "sp_Wms_PartMaterialRequestBom_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<PartMaterialRequestBomDetilItemDto>(sp, new { IDSeq = idSeq });
    }

    public async Task UpdateReq(PartMaterialRequestBomDetailModel payload, string userId)
    {
        string sql = @"sp_Wms_PartMaterialRequestBom_UpdateRequirement";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.IDSeq,
            payload.ChilItemCode,
            payload.ReqQty,
            UpdateBy = userId
        });
    }

    public async Task<Dictionary<string, object>> CaptureRequirement(long idSeq)
    {
        string sp = "sp_Wms_PartMaterialRequestBom_CaptureRequirement";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { IDSeq = idSeq });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }
}
