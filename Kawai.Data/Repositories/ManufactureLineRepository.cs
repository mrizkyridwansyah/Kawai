using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;

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
}
