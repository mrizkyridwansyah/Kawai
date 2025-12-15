using Kawai.Domain.DTOs;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobilePhysicalInventoryRepository
{
    Task<List<StockDto>> GetListStock(string addressCode, string userId);
    Task<StockDto> GetDataBarcode(string addressCode, string barcodeNo);
    Task Save(MobilePhysicalInventory payload, string userId);
    Task<Dictionary<string, object>> Capture(string barcodeNo);
}
