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
        string sp = "sp_Wms_Item_DDL";
        return (await _dbExecutor.QueryListAsync<ItemDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }
    public async Task<List<ItemDto>> DDLItemSearchByStock(string keyword, string warehouse, string area, string address, string category, string statusReceipt, string statusHoldNG)
    {
        string sp = "sp_Wms_Item_DDLByStock";
        return (await _dbExecutor.QueryListAsync<ItemDto>(sp,
            new
            {
                Keyword = keyword ?? "",
                WarehouseCode = !String.IsNullOrEmpty(warehouse) ? warehouse : "ALL",
                AreaCode = !String.IsNullOrEmpty(area) ? area : "ALL",
                AddressCode = !String.IsNullOrEmpty(address) ? address : "ALL",
                Category = !String.IsNullOrEmpty(category) ? category : "ALL",
                StatusReceipt = String.IsNullOrEmpty(statusReceipt) ? "ALL" : statusReceipt,
                StatusHoldNG = String.IsNullOrEmpty(statusHoldNG) ? "ALL" : statusHoldNG
            })).ToList();
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
            item.ItemCode,
            item.ItemName,
            item.FinishGoodPartCls,
            item.DrawingNumber,
            item.WarehouseCode,
            item.Address,
            item.SupplierCode,
            item.DeliveryPlaceCode,
            item.ManufactureCode,
            item.LineCode,
            item.MakerItemCode,
            item.PartCls,
            item.ReserveCls,
            item.SupplyCls,
            item.ProvisionCls,
            item.ProductionCls,
            item.MaterialCls,
            item.Thickness,
            item.Width,
            item.Length,
            item.Weight,
            item.GrossWeight,
            item.SheetCoilCls,
            item.Pitch,
            item.NumberProducible,
            item.ScrapWeight,
            item.DrawingMaterialCls,
            item.SurfaceTreatmentCls,
            item.SurfaceOrderPointQty,
            item.HeatTreatmentCls,
            item.HeatOrderPointQty,
            item.Sample,
            item.SWQty,
            item.EWQty,
            item.NumberProcess,
            item.MaterialCoefficient,
            item.ProcessCoefficient,
            item.MinLot,
            item.LotQty,
            item.LotCoefficience,
            item.ProductReadTime,
            item.YieldPercentage,
            item.NumberEntering,
            item.PackingStyleCls,
            item.GroupCls,
            item.StandardStock,
            item.SafetyStock,
            item.MaxStock,
            item.MinStock,
            item.AlowanceDay,
            item.DeliveryReadTime,
            item.MakeBuyCls,
            item.ControlCls,
            item.OrderPointQty,
            item.UnitCls,
            item.NumberBox,
            item.PackingStyleMaterialCls,
            item.AccountingCode,
            item.ExplosionCls,
            item.PersonInChargeCls,
            item.StockControlCls,
            item.SupplyIssueCls,
            item.UseEndDay,
            item.HSCode,
            item.MinOrder,
            item.SafetyStockPercentage,
            item.SAPItemCode,
            item.TypeAccs,
            item.ModelCls,
            item.POTypeCls,
            item.ClasificationPartCls,
            item.DestinationCls,
            item.ColorCls,
            RegisterBy = userId
        });
    }
    public async Task Update(Item item, string userId)
    {
        string sql = @"sp_Wms_Item_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            item.ItemCode,
            item.ItemName,
            item.FinishGoodPartCls,
            item.DrawingNumber,
            item.WarehouseCode,
            item.Address,
            item.SupplierCode,
            item.DeliveryPlaceCode,
            item.ManufactureCode,
            item.LineCode,
            item.MakerItemCode,
            item.PartCls,
            item.ReserveCls,
            item.SupplyCls,
            item.ProvisionCls,
            item.ProductionCls,
            item.MaterialCls,
            item.Thickness,
            item.Width,
            item.Length,
            item.Weight,
            item.GrossWeight,
            item.SheetCoilCls,
            item.Pitch,
            item.NumberProducible,
            item.ScrapWeight,
            item.DrawingMaterialCls,
            item.SurfaceTreatmentCls,
            item.SurfaceOrderPointQty,
            item.HeatTreatmentCls,
            item.HeatOrderPointQty,
            item.Sample,
            item.SWQty,
            item.EWQty,
            item.NumberProcess,
            item.MaterialCoefficient,
            item.ProcessCoefficient,
            item.MinLot,
            item.LotQty,
            item.LotCoefficience,
            item.ProductReadTime,
            item.YieldPercentage,
            item.NumberEntering,
            item.PackingStyleCls,
            item.GroupCls,
            item.StandardStock,
            item.SafetyStock,
            item.MaxStock,
            item.MinStock,
            item.AlowanceDay,
            item.DeliveryReadTime,
            item.MakeBuyCls,
            item.ControlCls,
            item.OrderPointQty,
            item.UnitCls,
            item.NumberBox,
            item.PackingStyleMaterialCls,
            item.AccountingCode,
            item.ExplosionCls,
            item.PersonInChargeCls,
            item.StockControlCls,
            item.SupplyIssueCls,
            item.UseEndDay,
            item.HSCode,
            item.MinOrder,
            item.SafetyStockPercentage,
            item.SAPItemCode,
            item.TypeAccs,
            item.ModelCls,
            item.POTypeCls,
            item.ClasificationPartCls,
            item.DestinationCls,
            item.ColorCls,
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
