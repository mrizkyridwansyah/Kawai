using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kawai.Data.Repositories.Mobile;

public class MobilePhysicalInventoryRepository : IMobilePhysicalInventoryRepository
{
    private readonly DbExecutor _dbExecutor;
    public MobilePhysicalInventoryRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<StockDto>> GetListStock(string addressCode, string userId)
    {
        string sp = "sp_Wms_Mobile_PhysicalInventory_GetListStock";
        return (await _dbExecutor.QueryListAsync<StockDto>(sp, new { AddressCode = addressCode, UserId = userId })).ToList();
    }

    public async Task<List<PhysicalInventorySummaryStockDto>> GetListStockSummary(string addressCode)
    {
        string sp = "sp_Wms_Mobile_PhysicalInventory_GetListStockSummary";
        return (await _dbExecutor.QueryListAsync<PhysicalInventorySummaryStockDto>(sp, new { AddressCode = addressCode })).ToList();
    }

    public async Task<List<StockDto>> GetListStockDetail(string addressCode, string itemCode)
    {
        string sp = "sp_Wms_Mobile_PhysicalInventory_GetListStock";
        return (await _dbExecutor.QueryListAsync<StockDto>(sp, new { AddressCode = addressCode, ItemCode = itemCode })).ToList();
    }

    public async Task<StockDto> GetDataBarcode(string addressCode, string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_PhysicalInventory_GetDataBarcode";
        return await _dbExecutor.QueryFirstOrDefaultAsync<StockDto>(sp, new { AddressCode = addressCode, BarcodeNo = barcodeNo });
    }
    public async Task Save(MobilePhysicalInventory payload, string userId)
    {
        string sql = "sp_Wms_Mobile_PhysicalInventory_Save";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.AddressCode,
            payload.BarcodeNo,
            InventoryQty = payload.InventoryQty ?? 0,
            UserId = userId
        });
    }

    public async Task<StockDto> GetDataBarcodeByWarehouse(string warehouseCode, string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_PhysicalInventory_GetDataBarcodeByWarehouse";
        return await _dbExecutor.QueryFirstOrDefaultAsync<StockDto>(sp, new { WarehouseCode = warehouseCode, BarcodeNo = barcodeNo });
    }
    public async Task SaveByWarehouse(MobilePhysicalInventoryWarehouse payload, string userId)
    {
        string sql = "sp_Wms_Mobile_PhysicalInventory_SaveByWarehouse";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            payload.WarehouseCode,
            payload.BarcodeNo,
            InventoryQty = payload.InventoryQty ?? 0,
            UserId = userId
        });
    }

    public async Task<Dictionary<string, object>> Capture(string barcodeNo)
    {
        string sp = "sp_Wms_Mobile_PhysicalInventory_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { BarcodeNo = barcodeNo });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }
}
