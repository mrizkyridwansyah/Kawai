
using Kawai.Domain.Interfaces;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;
using Kawai.Domain.Models;
using System.Net.Sockets;

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

    public async Task Update(List<PhysicalInventoryUpdateDto> model, string userId)
    {
        string sp = "sp_Wms_Physical_Inventory_Update";

        await _dbExecutor.ExecuteAsync(sp, new
        {
            LastUser = userId,
            Details = DataTableHelper.ToDataTable(model)
        });
    }

    public async Task<Dictionary<string, object>> Capture(List<PhysicalInventoryUpdateDto> model)
    {
        string sp = "sp_Wms_Physical_Inventory_Capture";
        var result = await _dbExecutor.QueryListAsync<PhysicalInventoryCaptureDto>(sp, new
        {
            Details = DataTableHelper.ToDataTable(model)
        });

        if (result == null)
            return new Dictionary<string, object>();

        return new Dictionary<string, object>
        {
            { "Detail", result }
        };
    }
}