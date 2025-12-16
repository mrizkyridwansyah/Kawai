using Kawai.Data.SqlConnections;
using Kawai.Domain.Interfaces;

namespace Kawai.Data.Repositories;

public class ImportRepository : IImportRepository
{
    private readonly DbExecutor _dbExecutor;

    public ImportRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<IDictionary<string, object>>> GetReferenceSheet(string sp)
    {
        var result = await _dbExecutor.QueryListAsync<dynamic>(sp);

        return result?
            .Select(r => (IDictionary<string, object>)r)
            .ToList()
            ?? new List<IDictionary<string, object>>();
        //return (await _dbExecutor.QueryListAsync<dynamic>(sp)).ToList();
    }

}
