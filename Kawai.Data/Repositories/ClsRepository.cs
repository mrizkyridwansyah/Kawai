using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;

namespace Kawai.Data.Repositories;

public class ClsRepository : IClsRepository
{
    private readonly DbExecutor _dbExecutor;

    public ClsRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ClsDto>> GetDDL(  string keyword ,string typedata)
    {
        string sp = "sp_Wms_Cls_DDL";
        return (await _dbExecutor.QueryListAsync<ClsDto>(sp, new { Keyword = keyword ?? "", TypeData = typedata })).ToList();
    }
}
