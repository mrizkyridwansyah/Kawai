using Kawai.Domain.DTOs;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileLoadingTrolleyRepository
{
    Task<LoadingTrolleyDto> GetDataTrolley(string trolleyNo);
    Task<List<StockDto>> GetDataBarcode(string trolleyNo, string barcodeNo);
    Task Save(MobileLoadingTrolley payload, string userId);
    Task<Dictionary<string, object>> Capture(string refNo);
}
