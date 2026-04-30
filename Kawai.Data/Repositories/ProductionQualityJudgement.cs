using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Dapper;
using System.Data;

namespace Kawai.Data.Repositories;

public class ProductionQualityJudgementRepository : IProductionQualityJudgementRepository
{

    private readonly DbExecutor _dbExecutor;

    public ProductionQualityJudgementRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ProductionQualityJudgementDto>> GetListHeader(RequestParameter param)
    {
        var paramPeriodFrom = param.GetParam("PeriodFrom");
        var paramPeriodUntil = param.GetParam("PeriodUntil");
        var paramFactory = param.GetParam("FactoryCode");
        var paramProcess = param.GetParam("ManufactureCode");
        var paramLine = param.GetParam("LineCode");
        var paramCompleteCls = param.GetParam("CompleteCls");

        string sp = "sp_Wms_Production_Quality_Judgement_List";

        return (await _dbExecutor.QueryListAsync<ProductionQualityJudgementDto>(sp, new
        {
            PeriodFrom = paramPeriodFrom,
            PeriodUntil = paramPeriodUntil,
            FactoryCode = paramFactory,
            ProcessCode = paramProcess,
            LineCode = paramLine,
            CompleteCls = paramCompleteCls == "ALL" ? (bool?)null : paramCompleteCls == "YES"
        })).ToList();
    }

    public async Task Save(List<ProductionQualityJudgementModel> models, string userId)
    {
        string sp = "sp_Wms_ProductionQualityJudgement_Save";

        var dt = new DataTable();
        dt.Columns.Add("ProductionId", typeof(long));
        dt.Columns.Add("ProdResultID", typeof(string));
        dt.Columns.Add("ResultType", typeof(string));
        dt.Columns.Add("ItemCode", typeof(string));

        foreach (var item in models)
        {
            dt.Rows.Add(
                item.ProductionId,
                item.ProdResultID,
                item.ResultType,
                item.ItemCode
            );
        }

        var parameters = new DynamicParameters();
        parameters.Add("@NewRequest", dt.AsTableValuedParameter("ProductionQualityJudgementType"));
        parameters.Add("@UserId", userId);

        await _dbExecutor.ExecuteAsync(sp, parameters);
    }

    public async Task<Dictionary<string, object>> Capture(long prodresultId)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_ProductionQualityJudgement_Capture",
            param: new { ProdResultID = prodresultId },
            async multi =>
            {
                var headers = (await multi.ReadAsync<dynamic>()).ToList();

                return headers;
            }
        );

        return new Dictionary<string, object>
        {
            { "Production Quality Judgement Input", result },
        };
    }


}
