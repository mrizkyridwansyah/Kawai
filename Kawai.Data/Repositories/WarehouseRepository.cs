using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly DbExecutor _dbExecutor;

    public WarehouseRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<WarehouseDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_Warehouse_List";
        return (await _dbExecutor.QueryListAsync<WarehouseDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<WarehouseDto> GetData(string warehouseCode)
    {
        string sp = "sp_Wms_Warehouse_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<WarehouseDto>(sp, new { WarehouseCode = warehouseCode });
    }

    public async Task<List<WarehouseDto>> GetDDL(string keyword)
    {
        string sp = "sp_Wms_Warehouse_DDL";
        return (await _dbExecutor.QueryListAsync<WarehouseDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

    public async Task<List<WarehouseDto>> DDLSearchByStock(string keyword, string item)
    {
        string sp = "sp_Wms_StockInquiry_DDLWarehouse";
        return (await _dbExecutor.QueryListAsync<WarehouseDto>(sp, new { Keyword = keyword ?? "", ItemCode = String.IsNullOrEmpty(item) ? "ALL" : item })).ToList();
    }

    public async Task Create(Warehouse warehouse, string userId)
    {
        string sql = @"sp_Wms_Warehouse_Create";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            warehouse.WarehouseName,
            warehouse.WarehouseCode,
            warehouse.AdmGroup,
            warehouse.StockControlCls,
            warehouse.UseEndDate,
            warehouse.NGCls,
            RegisterBy = userId
        });
    }

    public async Task Update(Warehouse warehouse, string userId)
    {
        string sql = @"sp_Wms_Warehouse_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            warehouse.WarehouseName,
            warehouse.WarehouseCode,
            warehouse.AdmGroup,
            warehouse.StockControlCls,
            warehouse.UseEndDate,
            warehouse.NGCls,
            UpdateBy = userId
        });
    }

    public async Task Remove(string warehouseCode, string userId)
    {
        string sql = "sp_Wms_Warehouse_Delete";
        int i = await _dbExecutor.ExecuteAsync(sql, new { WarehouseCode = warehouseCode });
    }

    public async Task<Dictionary<string, object>> Capture(string warehouseCode)
    {
        string sp = "sp_Wms_Warehouse_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { WarehouseCode = warehouseCode });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task<List<WarehousePrivilegesDto>> GetAllWarehouseIncludePrivileges(string userId)
    {
        string sp = "sp_WMS_UserSetup_UserPrivilegeWarehouse";
        return (await _dbExecutor.QueryListAsync<WarehousePrivilegesDto>(sp, new { UserID = userId })).ToList();
    }
}
