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
        var paramWarehouse = param.GetParam("WarehouseCode");
        var paramRemainingFuckingCls = param.GetParam("RemainingCls");

        string sp = "sp_Wms_PartMaterialRequestBom_GetListHeader";
        return (await _dbExecutor.QueryListAsync<PartMaterialRequestBomDto>(sp, new
        {
            PeriodFrom = paramPeriodFrom,
            PeriodUntil = paramPeriodUntil,
            FactoryCode = paramFactory,
            SupplierCode = paramSupplier,
            PONumber = paramPONumber,
            WarehouseCode = paramWarehouse,
            RemainingCls = paramRemainingFuckingCls == "ALL" ? (bool?)null : paramRemainingFuckingCls == "YES"
        })).ToList();
    }

    public async Task<List<PartMaterialRequestBomDetilDto>> GetListDetail(List<PartMaterialRequestBomModel> models)
    {
        string sp = "sp_Wms_PartMaterialRequestBom_GetListDetail";
        return (await _dbExecutor.QueryListAsync<PartMaterialRequestBomDetilDto>(sp, new
        {
            models[0].LineCode,
            NewRequest = DataTableHelper.ToDataTable(models, ["LineCode"])
        })).ToList();
    }
    public async Task<List<StockDto>> GetListStock(RequestParameter param)
    {
        string sp = "sp_Wms_PartMaterialRequest_GetListStock";
        return (await _dbExecutor.QueryListAsync<StockDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task Save(List<PartMaterialRequestBomModel> models, string userId)
    {
        string sp = "sp_Wms_PartMaterialRequestBom_Save";
        await _dbExecutor.ExecuteAsync(sp, new
        {
            models[0].LineCode,
            NewRequest = DataTableHelper.ToDataTable(models, ["LineCode"]),
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

    public async Task<Dictionary<string, object>> Capture(long productionId)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_PartMaterialRequestBom_Capture",
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
}
