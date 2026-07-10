using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class ManufactureLineRepository : IManufactureLineRepository
{
    private readonly DbExecutor _dbExecutor;
    public ManufactureLineRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }
    public async Task<List<ManufactureLineDto>> GetManufactureDDL(string keyword)
    {
        string sp = "sp_Wms_ManufactureLine_ManufactureDDL";
        return (await _dbExecutor.QueryListAsync<ManufactureLineDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

    public async Task<List<ManufactureLineDto>> GetLineDDL(string keyword, string manufactureCode)
    {
        string sp = "sp_Wms_ManufactureLine_LineDDL";
        return (await _dbExecutor.QueryListAsync<ManufactureLineDto>(sp, new { Keyword = keyword ?? "", ManufactureCode = manufactureCode })).ToList();
    }

    public async Task<List<ManufactureLineDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_ManufactureLine_List";
        return (await _dbExecutor.QueryListAsync<ManufactureLineDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<ManufactureLineDto> GetData(string linecode)
    {
        string sp = "sp_Wms_ManufactureLine_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ManufactureLineDto>(sp, new { LineCode = linecode });
    }

    public async Task Update(Manufactureline manufacture, string userId)
    {
        string sql = @"sp_Wms_ManufactureLine_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            manufacture.FactoryCode,
            manufacture.LineCode,
            manufacture.LineName,
            manufacture.IPPrinter, 
            UpdateBy = userId
        });
    }

    

    public async Task<Dictionary<string, object>> Capture(string linecode)
    {
        string sp = "sp_Wms_ManufactureLine_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { LineCode = linecode });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }


}
