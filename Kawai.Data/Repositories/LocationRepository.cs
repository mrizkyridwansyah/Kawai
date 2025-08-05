using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class LocationRepository : ILocationRepository
{
    private readonly DbExecutor _dbExecutor;

    public LocationRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<LocationDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_Location_List";
        return (await _dbExecutor.QueryListAsync<LocationDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<LocationDto> GetData(string locationCode)
    {
        string sp = "sp_Wms_Location_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<LocationDto>(sp, new { LocationCode = locationCode });
    }

    public async Task<List<LocationDto>> GetDDL(string keyword, string warehouseCode)
    {
        string sp = "sp_Wms_Location_DDL";
        return (await _dbExecutor.QueryListAsync<LocationDto>(sp, new { Keyword = keyword ?? "", WarehouseCode = warehouseCode })).ToList();
    }

    public async Task<List<LocationDto>> DDLSearchByStock(string keyword, string warehouseCode, string item)
    {
        string sp = "sp_Wms_StockInquiry_DDLLocation";
        return (await _dbExecutor.QueryListAsync<LocationDto>(sp, new { Keyword = keyword ?? "", WarehouseCode = warehouseCode, ItemCode = String.IsNullOrEmpty(item) ? "ALL" : item })).ToList();
    }

    public async Task Create(Location location, string userId)
    {
        string sql = @"sp_Wms_Location_Create";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            location.WarehouseCode,
            location.LocationCode,
            location.LocationName,
            RegisterBy = userId
        });
    }

    public async Task Update(Location location, string userId)
    {
        string sql = @"sp_Wms_Location_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            location.WarehouseCode,
            location.LocationCode,
            location.LocationName,
            UpdateBy = userId
        });
    }

    public async Task Remove(string locationCode, string userId)
    {
        string sql = "sp_Wms_Location_Delete";
        int i = await _dbExecutor.ExecuteAsync(sql, new { LocationCode = locationCode });
    }

    public async Task<Dictionary<string, object>> Capture(string locationCode)
    {
        string sp = "sp_Wms_Location_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { LocationCode = locationCode });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task<List<LocationPrivilegesDto>> GetAllLocationIncludePrivileges(string userId)
    {
        string sp = "sp_WMS_UserSetup_UserPrivilegeWarehouse";
        return (await _dbExecutor.QueryListAsync<LocationPrivilegesDto>(sp, new { UserID = userId })).ToList();
    }
}
