using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class ProductionUnscheduleRepository : IProductionUnscheduleRepository
{

    private readonly DbExecutor _dbExecutor;

    public ProductionUnscheduleRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ManufactureLineDto>> GetLineUnscheduleDDL(string keyword)
    {
        string sp = "sp_WMS_ProductionResultUnschedule_DDL";
        return (await _dbExecutor.QueryListAsync<ManufactureLineDto>(sp, new { Keyword = keyword ?? "", Type = "1"})).ToList();
    }

    public async Task<List<ParentItemUnscheduleDto>> GetParentItemUnscheduleDDL(string keyword)
    {
        string sp = "sp_WMS_ProductionResultUnschedule_DDL";
        return (await _dbExecutor.QueryListAsync<ParentItemUnscheduleDto>(sp, new { Keyword = keyword ?? "", Type = "2" })).ToList();

    }

    public async Task<List<ProductionUnscheduleBOMDto>> GetBOMRequirement(RequestParameter param)
    {
        var pLineCode = param.GetParam("LineCode");
        var pParentItemCode = param.GetParam("ParentItemCode");
        var pProductionDate = Convert.ToDateTime ( param.GetParam("ProductionDate"));
        var pQtyInput = Convert.ToDecimal (param.GetParam("QtyInput"));

        string sp = "sp_WMS_ProductionResultUnschedule_GetRequirementDetail";
        return (await _dbExecutor.QueryListAsync<ProductionUnscheduleBOMDto>(sp, new
        {
            LineCode = pLineCode,
            ParentItemCode = pParentItemCode,
            ProductionDate = pProductionDate,
            QtyInput = pQtyInput

        })).ToList();
    }

    public async Task<List<ProductionUnscheduleResultDto>> GetListResults(RequestParameter param)
    {
        var pLineCode = param.GetParam("LineCode");
        var pParentItemCode = param.GetParam("ParentItemCode");
        var pDateFrom = Convert.ToDateTime(param.GetParam("DateFrom"));
        var pDateTo = Convert.ToDateTime(param.GetParam("DateTo"));

        string sp = "sp_WMS_ProductionResultUnschedule_GetListData";
        return (await _dbExecutor.QueryListAsync<ProductionUnscheduleResultDto>(sp, new
        {
            LineCode = pLineCode,
            ParentItemCode = pParentItemCode,
            DateFrom = pDateFrom,
            DateTo = pDateTo
        })).ToList();
    }

    public async Task<List<ProductionUnscheduleDetailDto>> GetListResultsDetail(string id) //string ProdResultID) 
    {
       
        string sp = "sp_WMS_ProductionResultUnschedule_GetDetailResult";
        return (await _dbExecutor.QueryListAsync<ProductionUnscheduleDetailDto>(sp, new
        {
            ProdResultID = id
        })).ToList();
    }

    public async Task Save(ProductionUnscheduleModel models, string userId)
    {
       
        var QtyInput = Convert.ToDecimal(models.QtyInput);

        string sp = "sp_WMS_ProductionResultUnschedule_Submit";
        await _dbExecutor.ExecuteAsync(sp, new
        {
            LineCode=models.LineCode,
            ParentItemCode = models.ParentItemCode,
            ProductionDate = models.ProductionDate,
            QtyInput = QtyInput,
            Remarks = models.Remarks,
            UserId = userId
        });
    }

    

    //public async Task<Dictionary<string, object>> Capture(long productionId)
    //{
    //    var result = await _dbExecutor.QueryMultipleAsync(
    //        "sp_Wms_ProductionResultManualInput_Capture",
    //        param: new { ProductionId = productionId },
    //        async multi =>
    //        {
    //            var headers = (await multi.ReadAsync<dynamic>()).ToList();
    //            var details = (await multi.ReadAsync<dynamic>()).ToList();
    //            var detailItems = (await multi.ReadAsync<dynamic>()).ToList();

    //            foreach (var header in headers)
    //            {
    //                header.Details = details.Where(p => p.RequestID == header.RequestID).ToList();
    //                header.DetailItems = detailItems.Where(p => p.RequestID == header.RequestID).ToList();
    //            }

    //            return headers;
    //        }
    //    );

    //    return new Dictionary<string, object>
    //    {
    //        { "Production Result Manual Input", result },
    //    };
    //}

   
}
