using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileBarcodeSplitRepository
{
    Task<BarcodeSplitDto> GetDataBarcode(string barcodeNo);
    Task<List<BarcodeSplitHistoryDto>> GetHistorySplit(string barcodeNo);
    Task Save(MobileBarcodeSplit payload, string userId);
    Task<Dictionary<string, object>> Capture(string barcodeNo);

}
