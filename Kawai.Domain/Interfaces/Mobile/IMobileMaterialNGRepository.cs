using Kawai.Domain.DTOs;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileMaterialNGRepository
{
    Task<MaterialNGDto> GetDataNGBarcode(string barcodeNo);
    Task Save(MobileMaterialNG payload, string userId);
    Task<Dictionary<string, object>> Capture(long inspectionId);
}
