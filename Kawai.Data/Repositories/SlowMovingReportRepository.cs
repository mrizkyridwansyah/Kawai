using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class SlowMovingReportRepository : ISlowMovingReportRepository
{
    private readonly DbExecutor _dbExecutor;

    public SlowMovingReportRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<SlowMovingReportDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_SlowMovingReport_List";
        var paramObj = param.ToQueryObject();

        var rows = await _dbExecutor.QueryListAsync<dynamic>(sp, paramObj);

        var result = rows.Select(r =>
        {
            var dict = r as IDictionary<string, object>;
            if (dict == null)
                return null;

            var dto = new SlowMovingReportDto
            {
                LongStock = dict["LongStock"]?.ToString().Trim(),
                Item_Code = dict["Item_Code"]?.ToString().Trim(),
                Item_Name = dict["Item_Name"]?.ToString().Trim(),
                Unit_Name = dict["Unit_Name"]?.ToString().Trim(),
                WHCode = dict["WHCode"]?.ToString().Trim(),
                WHName = dict["WHName"]?.ToString().Trim(),
                Remarks = dict["Remarks"]?.ToString().Trim(),
            };

            // kolom pivot dynamic (YYYYMM)
            foreach (var kv in dict)
            {
                if (kv.Key.Length == 6 && kv.Key.All(char.IsDigit))
                {
                    dto.PeriodQty[kv.Key] =
                        kv.Value == null ? null : Convert.ToDecimal(kv.Value);
                }
            }

            return dto;
        })
        .Where(x => x != null)
        .ToList();

        return result;

        //return (await _dbExecutor.QueryListAsync<SlowMovingReportDto>(sp, paramObj)).ToList();
    }

}