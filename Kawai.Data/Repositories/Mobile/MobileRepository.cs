using Kawai.Data.SqlConnections;
using Kawai.Domain.Interfaces.Mobile;

namespace Kawai.Data.Repositories.Mobile;

public class MobileRepository: IMobileRepository
{
    private readonly DbExecutor _dbExecutor;
    public MobileRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }
    public async Task<string> GetLastVersion()
    {
        return await _dbExecutor.QueryFirstOrDefaultAsync<string>("sp_Wms_Mobile_GetLastVersion");
    }
}
