using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using System.Reflection;
using System.Reflection.PortableExecutable;

namespace Kawai.Data.Repositories;

public class ItemPackingSupplierRepository : IItemPackingSupplierRepository
{
    private DbExecutor _dbExecutor;
    public ItemPackingSupplierRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ItemPackingSupplierDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_ItemPackingSupplier_List";
        return (await _dbExecutor.QueryListAsync<ItemPackingSupplierDto>(sp, param.ToQueryObject())).ToList();
    }
    public async Task<ItemPackingSupplierDto> GetData(string supplierCode, string itemCode)
    {
        string sp = "sp_Wms_ItemPackingSupplier_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ItemPackingSupplierDto>(sp, new { SupplierCode = supplierCode, ItemCode = itemCode });
    }
    public async Task Create(ItemPackingSupplier model, string userId)
    {
        string sql = @"sp_Wms_ItemPackingSupplier_Create";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            model.SupplierCode,
            model.ItemCode,
            model.QtyPacking,
            RegisterBy = userId
        });
    }
    public async Task Update(ItemPackingSupplier model, string userId)
    {
        string sql = @"sp_Wms_ItemPackingSupplier_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            model.SupplierCode,
            model.PrevItemCode,
            model.ItemCode,
            model.QtyPacking,
            UpdateBy = userId
        });
    }
    public async Task Remove(string supplierCode, string itemCode)
    {
        string sql = "sp_Wms_ItemPackingSupplier_Delete";
        int i = await _dbExecutor.ExecuteAsync(sql, new { SupplierCode = supplierCode, ItemCode = itemCode });
    }
    public async Task<Dictionary<string, object>> Capture(string supplierCode, string itemCode)
    {
        string sp = "sp_Wms_ItemPackingSupplier_Capture";
        var result = (await _dbExecutor.QueryListAsync<dynamic>(sp, new { SupplierCode = supplierCode, ItemCode = itemCode })).ToList();

        return new Dictionary<string, object>
        {
            { "List", result }
        };

    }

}
