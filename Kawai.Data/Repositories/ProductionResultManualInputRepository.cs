using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class ProductionResultManualInputRepository : IProductionResultManualInputRepository
{

    private readonly DbExecutor _dbExecutor;

    public ProductionResultManualInputRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ProductionResultManualInputDto>> GetListHeader(RequestParameter param)
    {
        var paramPeriodFrom = param.GetParam("PeriodFrom");
        var paramPeriodUntil = param.GetParam("PeriodUntil");
        var paramFactory = param.GetParam("FactoryCode");
        var paramProcess = param.GetParam("ManufactureCode");
        var paramLine = param.GetParam("LineCode");
        var paramCompleteCls = param.GetParam("CompleteCls");

        string sp = "sp_Wms_ProductionResultManualInput_GetListHeader";
        return (await _dbExecutor.QueryListAsync<ProductionResultManualInputDto>(sp, new
        {
            PeriodFrom = paramPeriodFrom,
            PeriodUntil = paramPeriodUntil,
            FactoryCode = paramFactory,
            ProcessCode = paramProcess,
            LineCode = paramLine,
            CompleteCls = paramCompleteCls == "ALL" ? (bool?)null : paramCompleteCls == "YES"
        })).ToList();
    }

    public async Task<List<ProductionResultManualInputDetailDto>> GetListDetail(List<ProductionResultManualInputModel> models)
    {
        string sp = "sp_Wms_ProductionResultManualInput_GetListDetail";
        return (await _dbExecutor.QueryListAsync<ProductionResultManualInputDetailDto>(sp, new
        {
            models[0].LineCode,
            NewRequest = DataTableHelper.ToDataTable(models, ["LineCode"])
        })).ToList();
    }
    public async Task<List<StockDto>> GetListStock(RequestParameter param)
    {
        string sp = "sp_Wms_ProductionResultManualInput_GetListStock";
        return (await _dbExecutor.QueryListAsync<StockDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task Save(List<ProductionResultManualInputModel> models, string userId)
    {
        string sp = "sp_Wms_ProductionResultManualInput_Save";
        await _dbExecutor.ExecuteAsync(sp, new
        {
            models[0].LineCode,
            NewRequest = DataTableHelper.ToDataTable(models, ["LineCode"]),
            UserId = userId
        });
    }

    

    public async Task<Dictionary<string, object>> Capture(long productionId)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_ProductionResultManualInput_Capture",
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
            { "Production Result Manual Input", result },
        };
    }

   
}
