using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;

namespace Kawai.Data.Repositories;

public class ProdMaterialRequirementRepository : IProdMaterialRequirementRepository
{
    private readonly DbExecutor _dbExecutor;

    public ProdMaterialRequirementRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<ProdMaterialRequirementDto> GetLastCalculation(string factory)
    {
        string sp = "sp_Wms_ProdMaterialRequirement_GetLastCalculate";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ProdMaterialRequirementDto>(sp, new { Factory = factory });
    }

    public async Task<List<ProdMaterialRequirementDetailDto>> GetListDetail(string factory)
    {
        string sp = "sp_Wms_ProdMaterialRequirement_GetListDetail";
        return (await _dbExecutor.QueryListAsync<ProdMaterialRequirementDetailDto>(sp, new
        {
            Factory = factory,
        })).ToList();

    }

    public async Task Save(ProdMaterialRequirement model, string userId)
    {
        string sp = "sp_Wms_ProdMaterialRequirement_Calculate";
        await _dbExecutor.ExecuteAsync(sp, new
        {
            model.ParamKey,
            model.Factory,
            model.Process,
            model.Line,
            model.Model,
            model.ScheduleDateTo,
            UserId = userId
        });
    }

    public async Task<Dictionary<string, object>> Capture(string paramKey)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_ProdMaterialRequirement_Capture",
            param: new { ParamKey = paramKey },
            async multi =>
            {
                var header = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var details = (await multi.ReadAsync<dynamic>()).ToList();

                header.Materials = details.GroupBy(x => new { x.ParamKey, x.ChildItemCode, x.UnitCls, x.TotalReqQty, x.CurrentStock, x.Shortage })
                .Select(g => new
                {
                    g.Key.ParamKey,
                    g.Key.ChildItemCode,
                    g.Key.UnitCls,
                    g.Key.TotalReqQty,
                    g.Key.CurrentStock,
                    g.Key.Shortage,
                    Details = g.Select(x => new
                    {
                        x.ProductionId,
                        x.Process,
                        x.Line,
                        x.ParentItemCode,
                        x.ScheduleDate,
                        x.ReqQty,
                        x.ScanQty,
                        x.FinalReqQty
                    }).ToList()
                }).ToList();
                return (header);
            }
        );

        return new Dictionary<string, object>
        {
            { "Last Calculation", result },
        };
    }

}
