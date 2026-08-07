using Kawai.Domain.DTOs;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileProductionResultScanPalletizerRepository
{
    Task<MobileProductionResultScanDto> GetDataBarcode(string barcodeNo);
    Task Save(MobileProductionResultSubmit payload, string userId);
    Task<Dictionary<string, object>> Capture(string barcodeNo);
}
