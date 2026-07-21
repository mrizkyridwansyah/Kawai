using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class AndonFilterRepository : IAndonFilterRepository
{

    private readonly DbExecutor _dbExecutor;

    public AndonFilterRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<AndonFilterDto>> DDLArea(string keyword, string warehouseCode)
    {
        string sp = "sp_Wms_Andon_Filter_DDL";
        return (await _dbExecutor.QueryListAsync<AndonFilterDto>(sp, new { Keyword = keyword ?? "", WarehouseCode = warehouseCode })).ToList();
    }
    public async Task<List<AndonFilterDto>> DDLStatus(string keyword)
    {
        string sp = "sp_Wms_Andon_FilterStatus_DDL";
        return (await _dbExecutor.QueryListAsync<AndonFilterDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

    public async Task<List<AndonFilterDto>> DDLLine(string keyword)
    {
        string sp = "sp_Wms_Andon_FilterLine_DDL";
        return (await _dbExecutor.QueryListAsync<AndonFilterDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

    public async Task<List<AndonFilterDto>> DDLModel(string keyword)
    {
        string sp = "sp_Wms_Andon_FilterModel_DDL";
        return (await _dbExecutor.QueryListAsync<AndonFilterDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

    public async Task<List<AndonFilterDto>> DDLSupplier(string keyword)
    {
        string sp = "sp_Wms_Andon_FilterSupplier_DDL";
        return (await _dbExecutor.QueryListAsync<AndonFilterDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

}
