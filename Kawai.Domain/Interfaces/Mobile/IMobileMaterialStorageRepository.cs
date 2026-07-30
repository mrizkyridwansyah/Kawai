using Kawai.Domain.DTOs;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileMaterialStorageRepository
{
    Task<List<MaterialStorageSummaryDto>> GetSummaryStorage(string warehouseCode, string barcode);
    Task<List<MaterialStorageDto>> GetListDetail(string refNo, string warehouseCode, string lotNo, string itemCode);
    Task<MaterialStorageDto> GetDataBarcode(string barcodeNo, bool onlyTemp);
    Task Save(MobileMaterialStorage payload, string userId);
    Task SaveMerge(MobileMaterialMergeStorage payload, string userId);
    Task<Dictionary<string, object>> Capture(string refNo);

    Task<List<MaterialStorageDto>> GetListRemainingStock(string userId);
}
