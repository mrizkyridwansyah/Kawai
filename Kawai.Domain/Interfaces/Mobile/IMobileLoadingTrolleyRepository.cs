using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileLoadingTrolleyRepository
{
    Task<List<LoadingTrolleyDto>> GetDataTrolley(string trolleyNo);
    Task<List<LoadingTrolleyDto>> GetListRouteTrolley(string trolleyNo);
    Task ScanBarcode(MobileLoadingTrolley payload, string userId);
    Task CompleteLoading(MobileLoadingTrolleyComplete payload, string userId);
    Task<Dictionary<string, object>> Capture(string refNo);
    Task<Dictionary<string, object>> CapturePicking(string pickingNo);
}
