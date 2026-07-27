using Kawai.Domain.DTOs;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobilePhysicalInventoryRepository
{
    Task<List<StockDto>> GetListStock(string addressCode, string userId);
    Task<List<PhysicalInventorySummaryStockDto>> GetListStockSummary(string addressCode);
    Task<List<StockDto>> GetListStockDetail(string addressCode, string itemCode);
    Task<StockDto> GetDataBarcode(string addressCode, string barcodeNo);
    Task<StockDto> GetDataBarcodeByWarehouse(string warehouseCode, string barcodeNo);
    Task Save(MobilePhysicalInventory payload, string userId);
    Task SaveByWarehouse(MobilePhysicalInventoryWarehouse payload, string userId);
    Task<Dictionary<string, object>> Capture(string barcodeNo);
}
