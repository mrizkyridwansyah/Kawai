using Kawai.Domain.DTOs;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileSupplyScanRequestRepository
{
    Task<List<SupplyScanRequestNoDto>> GetRequestNoDDL(string keyword, string factoryCode);
    Task<SupplyScanRequestDto> GetDataBarcode(string barcodeNo);
}
