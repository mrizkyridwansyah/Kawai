using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileManualTrolleyAssignRepository
{
    Task<List<ManufactureLineDto>> GetLineDDL(string keyword,string itemClass);
    Task<List<SupplyScanRequestNoDto>> GetRequestNoDDL(string keyword, string lineCode, string itemClass);
    Task<TrolleyDto> GetDataTrolley(string trolleyNo);
    Task Save(MobileManualTrolleyAssign payload, string userId);
    Task<Dictionary<string, object>> Capture(string requestNo);

    #region AMR
    //Task UpdateStatusAMR(string requestNo, string lastStatus);
    //Task<Dictionary<string, object>> CaptureStatusAMR(string requestNo);
    #endregion
}
