using Kawai.Domain.DTOs;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileSupplySubconRepository
{
    Task<List<SupplySubconRequestNoDto>> GetRequestNoDDL(string keyword, string itemClass , string supplierCode);
    Task<SupplySubconDto> GetDataBarcode(string barcodeNo, string requestNo, string itemClass);
    Task<List<SupplySubconDto>> GetListDetailMaterial(string requestno, string itemClass);
    Task<List<StockDto>> GetListStock(string itemCode);
    Task Save(MobileSupplySubcon payload, string userId);
    Task<Dictionary<string, object>> Capture(string requestNo, string itemCode);
}
