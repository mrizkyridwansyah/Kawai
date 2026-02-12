using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class ReprintRepository : IReprintRepository
{
    private readonly DbExecutor _dbExecutor;

    public ReprintRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ReprintDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_Reprint_List";
        return (await _dbExecutor.QueryListAsync<ReprintDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task UpdatePrintValue(string keyData, string valueData, string userId)
    {
        string sql = "sp_Wms_Reprint_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new { BarcodeNo = keyData , Source = valueData , UserID  = userId });
    }

}
