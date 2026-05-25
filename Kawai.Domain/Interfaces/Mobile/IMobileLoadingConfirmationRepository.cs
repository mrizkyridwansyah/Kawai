using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileLoadingConfirmationRepository
{
    Task<List<LoadingConfirmationInstructionDto>> GetInstructionDDL(string keyword);
    Task<List<LoadingConfirmationListDto>> GetListDetailShipping(string instructionNo, string keyword);
    Task<LoadingConfirmationDetailDto> GetDataBarcode(string barcodeNo, string instructionNo);
    Task<List<LoadingConfirmationDetailDto>> GetListDetail(string instructionNo, string barcodeNo, string partNo, string serialNo);
    Task<bool> Save(MobileLoadingConfirmationSubmit payLoad, string deviceId, string userId);
    Task<Dictionary<string, object>> Capture(string instructionNo, string barcodeNo);

    Task<bool> SubmitEvidenceBefore(MobileLoadingConfirmationEvidenceBeforeQueueSubmit payload, string userId);
    Task<bool> SubmitEvidenceAfter(MobileLoadingConfirmationEvidenceAfterQueueSubmit payload, string userId);
    Task<Dictionary<string, object>> CaptureEvidence(string instructionNo);

    Task<LoadingConfirmationEvidenceBeforeDetailDto> GetEvidenceBeforeDetail(string instructionNo);
    Task<LoadingConfirmationEvidenceAfterDetailDto> GetEvidenceAfterDetail(string instructionNo);
}