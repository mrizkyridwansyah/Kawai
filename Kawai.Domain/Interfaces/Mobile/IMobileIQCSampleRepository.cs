using Kawai.Domain.DTOs;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileIQCSampleRepository
{
    Task<List<IQCInspectionDto>> GetListSample(long receiptId);
    Task<IQCSampleDetailBarcodeDto> GetDataSampleBarcode(long receiptId, string barcodeNo);
    Task Save(MobileIQCSample payload, string userId);
    Task<Dictionary<string, object>> Capture(long inspectionId);
}
