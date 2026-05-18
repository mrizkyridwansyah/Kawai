using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobilePickingByScanRepository
{
    Task<List<PickingByScanInstructionDto>> GetInstructionDDL(string keyword);
    Task<List<PickingByScanListDto>> GetListDetailShipping(string instructionNo, string keyword);
    Task<PickingByScanDetailDto> GetDataBarcode(string barcodeNo, string instructionNo);
    Task<List<PickingByScanDetailDto>> GetListDetail(string instructionNo, string barcodeNo, string partNo, string serialNo);
    Task<bool> Save(MobilePickingByScanSubmit payLoad, string deviceId, string userId);
    Task<Dictionary<string, object>> Capture(string instructionNo, string barcodeNo);
}