using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;

namespace Kawai.Data.Repositories;

public class HSRepository : IHSRepository
{
    private readonly DbExecutor _dbExecutor;
    public HSRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }
    public async Task<List<HSDto>> GetDDL(string keyword)
    {
        string sp = "sp_Wms_HS_DDL";
        return (await _dbExecutor.QueryListAsync<HSDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }
}
