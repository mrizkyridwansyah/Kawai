using Kawai.Domain.DTOs.Robot;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileEnableAMRModeRepository
{
    Task<SupplyRequestDto> GetDataTrolley(string trolleyNo);
    Task Save(MobileManualTrolleyAssign payload, string userId);
    Task<Dictionary<string, object>> Capture(string requestNo);

    #region AMR
    //Task UpdateStatusAMR(string requestNo, string lastStatus);
    //Task<Dictionary<string, object>> CaptureStatusAMR(string requestNo);
    #endregion
}
