using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.DTOs.Robot;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Models.Robot;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileLoadingTrolleyRepository
{
    Task<List<LoadingTrolleyDto>> GetDataTrolley(string trolleyNo);
    Task ScanBarcode(MobileLoadingTrolley payload, string userId);
    Task<CompleteStatusRequest> CompleteLoading(MobileLoadingTrolleyComplete payload, string userId);
    Task<Dictionary<string, object>> Capture(string refNo);
    Task<Dictionary<string, object>> CapturePicking(string pickingNo);

    Task<List<SupplyRequestCompleteDto>> GetListRouteTrolley(string trolleyNo);

    #region AMR
    Task UpdateStatusAMR(string pickingNo, string stopPoint, string lastStatus);
    Task SendRequestCompleteStatusAMR(string pickingNo, string stopPoint, string userId);
    Task<Dictionary<string, object>> CaptureStatusAMR(string pickingNo, string stopPoint);
    #endregion
}
