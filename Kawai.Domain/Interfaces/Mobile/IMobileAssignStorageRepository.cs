using Kawai.Domain.DTOs;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileAssignStorageRepository
{
    Task<List<StockDto>> GetDataBarcode(string barcodeNo);
    Task Save(MobileAssignStorage payload, string userId);
    Task<Dictionary<string, object>> Capture(string refNo);
}
