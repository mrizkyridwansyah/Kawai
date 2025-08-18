using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
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

}
