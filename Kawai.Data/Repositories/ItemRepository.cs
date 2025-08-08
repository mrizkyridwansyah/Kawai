using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class ItemRepository : IItemRepository
{
    private DbExecutor _dbExecutor;
    public ItemRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ItemDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_Item_List";
        return (await _dbExecutor.QueryListAsync<ItemDto>(sp, param.ToQueryObject())).ToList();
    }
    public async Task<List<ItemDto>> GetDDL(string keyword)
    {
        string sp = "sp_Wms_Warehouse_DDL";
        return (await _dbExecutor.QueryListAsync<ItemDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }
    public async Task<List<WarehouseDto>> GetWarehouseDDL(string keyword)
    {
        string sp = "sp_Wms_ItemWarehouse_DDL";
        return (await _dbExecutor.QueryListAsync<WarehouseDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }
    public async Task<ItemDto> GetData(string itemCode)
    {
        string sp = "sp_Wms_Item_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ItemDto>(sp, new { ItemCode = itemCode });
    }
    public async Task Create(Item item, string userId)
    {
        string sql = @"sp_Wms_Item_Create";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            item,
            RegisterBy = userId
        });
    }
    public async Task Update(Item item, string userId)
    {
        string sql = @"sp_Wms_Item_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            item,
            UpdateBy = userId
        });
    }
    public async Task Remove(string itemCode)
    {
        string sql = "sp_Wms_Item_Delete";
        int i = await _dbExecutor.ExecuteAsync(sql, new { ItemCode = itemCode });
    }
    public async Task<Dictionary<string, object>> Capture(string itemCode)
    {
        string sp = "sp_Wms_Item_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { ItemCode = itemCode });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);

    }

}
