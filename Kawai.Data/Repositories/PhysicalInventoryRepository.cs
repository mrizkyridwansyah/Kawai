
using Kawai.Domain.Interfaces;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;
using Kawai.Domain.Models;

namespace Kawai.Data.Repositories;

public class PhysicalInventoryRepository : IPhysicalInventoryRepository
{
    private readonly DbExecutor _dbExecutor;

    public PhysicalInventoryRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<PhysicalInventoryDto>> GetList(RequestParameter param)
    {
        string sp = "sp_Wms_Physical_Inventory_List";
        var paramQuery = param.ToQueryObject();

        return (await _dbExecutor.QueryListAsync<PhysicalInventoryDto>(sp, paramQuery)).ToList();
    }

    public async Task Update(PhysicalInventoryUpdateDto model, string userId)
    {
        string sp = "sp_Wms_Physical_Inventory_Update";

        await _dbExecutor.ExecuteAsync(sp, new
        {
            model.WarehouseCode,
            ItemCode = model.ProductCode,
            model.Period,
            model.Inventory,
            model.Reason,
            LastUser = userId
        });
    }

    public async Task<Dictionary<string, object>> Capture(string warehouseCode, string itemCode)
    {
        string sp = "sp_Wms_Physical_Inventory_Capture";

        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(
            sp, new
            {
                WarehouseCode = warehouseCode,
                ItemCode = itemCode
            }
        );

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);

    }
}