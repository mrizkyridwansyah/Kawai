using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileSupplyScanRequestRepository
{
    Task<List<SupplyScanRequestNoDto>> GetRequestNoDDL(string keyword, string factoryCode , string warehouse);
    Task<SupplyScanRequestDto> GetDataBarcode(string barcodeNo, string requestNo, string itemClass);
    Task<List<SupplyScanRequestDto>> GetListDetailMaterial(string requestno, string itemClass);
    Task<List<WarehouseDto>> GetWarehouseDDL(string keyword);
    Task<List<ManufactureLineDto>> GetLineDDL(string keyword,string warehousecode);
    Task<List<SupplyScanRequestDetailDto>> GetListDetail(string warehouseCode, string requestNo, string itemCode);
    Task Save(MobilSupplyScanRequestSubmit payload, string userId);
    Task<Dictionary<string, object>> Capture(string barcodeNo, string pickingNo, string itemClass);
}
