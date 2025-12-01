using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class PORepository : IPORepository
{

    private readonly DbExecutor _dbExecutor;

    public PORepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<PODto>> GetList(RequestParameter param)
    {
        string sp = "sp_Wms_PO_List";
        return (await _dbExecutor.QueryListAsync<PODto>(sp, param.ToQueryObject())).ToList();
    }
    public async Task<PODto> GetDetail(string poNumber)
    {
        string sp = "sp_Wms_PO_Detail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<PODto>(sp, new { PONumber = poNumber });
    }

    public async Task<List<PODetailDto>> GetListDetail(RequestParameter param)
    {
        string sp = "sp_Wms_PO_ListDetail";
        return (await _dbExecutor.QueryListAsync<PODetailDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<Dictionary<string, object>> Capture(string poNumber)
    {
        string sp = "sp_Wms_PO_CaptureHeader";
        var header = await _dbExecutor.QueryFirstOrDefaultAsync<PODto>(sp, new { PONumber = poNumber });

        string spDetail = "sp_Wms_PO_CaptureListDetail";
        var detail = (await _dbExecutor.QueryListAsync<PODto>(spDetail, new { PONumber = poNumber })).ToList();

        return new Dictionary<string, object>
        {
            { "Header", header },
            { "Detail", detail }
        };
    }

    public async Task<List<PODto>> GetDDL(string keyword, string factory, string supplier, string typeDate, DateTime? periodFrom, DateTime? periodUntil, bool showOptionAll, string userId)
    {
        string sp = "sp_Wms_PO_DDL";
        var today = DateTime.Today;
        var awalBulan = new DateTime(today.Year, today.Month, 1);

        return (await _dbExecutor.QueryListAsync<PODto>(sp, new
        {
            Keyword = keyword ?? "",
            FactoryCode = String.IsNullOrEmpty(factory) ? "ALL" : factory,
            SupplierCode = String.IsNullOrEmpty(supplier) ? "ALL" : supplier,
            TypeDate = typeDate,
            PeriodFrom = periodFrom.HasValue ? periodFrom.Value : awalBulan,
            PeriodUntil = periodUntil.HasValue ? periodUntil.Value : DateTime.Today,
            ShowOptionAll = showOptionAll,
            UserId = userId
        })).ToList();
    }

}
