using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class AreaRepository : IAreaRepository
{
    private readonly DbExecutor _dbExecutor;

    public AreaRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<AreaDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_Area_List";
        return (await _dbExecutor.QueryListAsync<AreaDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<AreaDto> GetData(string areaCode)
    {
        string sp = "sp_Wms_Area_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<AreaDto>(sp, new { AreaCode = areaCode });
    }

    public async Task<List<AreaDto>> GetDDL(string keyword)
    {
        string sp = "sp_Wms_Area_DDL";
        return (await _dbExecutor.QueryListAsync<AreaDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

    public async Task<List<AreaDto>> DDLSearchByStock(string keyword, string item)
    {
        string sp = "sp_Wms_StockInquiry_DDLArea";
        return (await _dbExecutor.QueryListAsync<AreaDto>(sp, new { Keyword = keyword ?? "", ItemCode = String.IsNullOrEmpty(item) ? "ALL" : item })).ToList();
    }

    public async Task Create(Area area, string userId)
    {
        string sql = @"sp_Wms_Area_Create";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            area.WarehouseCode,
            area.LocationCode,
            area.AreaCode,
            area.AreaName,
            RegisterBy = userId
        });
    }

    public async Task Update(Area area, string userId)
    {
        string sql = @"sp_Wms_Area_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            area.WarehouseCode,
            area.LocationCode,
            area.AreaCode,
            area.AreaName,
            UpdateBy = userId
        });
    }

    public async Task Remove(string areaCode, string userId)
    {
        string sql = "sp_Wms_Area_Delete";
        int i = await _dbExecutor.ExecuteAsync(sql, new { AreaCode = areaCode });
    }

    public async Task<Dictionary<string, object>> Capture(string areaCode)
    {
        string sp = "sp_Wms_Area_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { AreaCode = areaCode });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task<List<AreaPrivilegesDto>> GetAllAreaIncludePrivileges(string userId)
    {
        string sp = "sp_WMS_UserSetup_UserPrivilegeArea";
        return (await _dbExecutor.QueryListAsync<AreaPrivilegesDto>(sp, new { UserID = userId })).ToList();
    }
}
