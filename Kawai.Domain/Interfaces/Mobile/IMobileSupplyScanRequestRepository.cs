using Kawai.Domain.DTOs;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileSupplyScanRequestRepository
{
    Task<List<SupplyScanRequestNoDto>> GetRequestNoDDL(string keyword, string factoryCode);
    Task<SupplyScanRequestDto> GetDataBarcode(string barcodeNo);
    Task<List<SupplyScanRequestDto>> GetListDetailMaterial(string requestno);
    Task<List<SupplyScanRequestDetailDto>> GetListDetail(string warehouseCode, string requestNo, string itemCode);
    Task Save(MobilSupplyScanRequest payload, string userId);
    Task<Dictionary<string, object>> Capture(string barcodeNo);
}
