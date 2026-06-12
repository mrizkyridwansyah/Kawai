using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileManualTrolleyAssignRepository
{
    Task<List<ManufactureLineDto>> GetLineDDL(string keyword,string itemClass);
    Task<List<SupplyScanRequestNoDto>> GetRequestNoDDL(string keyword, string itemClass, string lineCode);
    Task<ManualTrolleyAssignDto> GetDataRequest(string requestNo);
    Task<TrolleyDto> GetDataTrolley(string requestNo, string trolleyNo);
    Task<ManualTrolleyAssignValidationDto> CheckValidation(MobileManualTrolleyAssign payload);
    Task Save(MobileManualTrolleyAssign payload, string userId);
    Task<Dictionary<string, object>> Capture(string requestNo);


    #region AMR
    Task<List<ManualTrolleyDetailRequestDto>> GetListDetailRequestAMR(string requestNo);
    Task SendRequestCancelAMR(string requestNo, string trolleyNo, string userId);
    Task UpdateStatusAMR(string requestNo, string trolleyNo, string lastStatus);
    Task<Dictionary<string, object>> CaptureStatusAMR(string pickingNo);

    #endregion
}
