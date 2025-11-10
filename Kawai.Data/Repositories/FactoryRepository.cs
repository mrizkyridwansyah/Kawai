using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;

namespace Kawai.Data.Repositories;

public class FactoryRepository : IFactoryRepository
{
    private readonly DbExecutor _dbExecutor;

    public FactoryRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<FactoryDto>> GetDDL(string keyword)
    {
        string sp = "sp_Wms_Factory_DDL";
        return (await _dbExecutor.QueryListAsync<FactoryDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

    public async Task<List<FactoryDto>> GetDDLPrivileges(string keyword, string userId)
    {
        string sp = "sp_Wms_FactoryPrivileges_DDL";
        return (await _dbExecutor.QueryListAsync<FactoryDto>(sp, new { Keyword = keyword ?? "", UserId = userId })).ToList();
    }

    public async Task<List<FactoryPrivilegesDto>> GetAllFactoryIncludePrivileges(string userId)
    {
        string sp = "sp_WMS_UserSetup_UserPrivilegeFactory";
        return (await _dbExecutor.QueryListAsync<FactoryPrivilegesDto>(sp, new { UserID = userId })).ToList();
    }
}
