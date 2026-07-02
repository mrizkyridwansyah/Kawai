using Kawai.Domain.DTOs.Mobile;


namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileBarcodeTraceabilityRepository
{
    Task<BarcodeTraceabilityDto> GetDataBarcode(string barcodeNo);
}
