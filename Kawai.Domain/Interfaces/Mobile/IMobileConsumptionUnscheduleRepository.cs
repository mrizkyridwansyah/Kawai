using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileConsumptionUnscheduleRepository
{
    Task<ConsumpUnscheduleDto> GetDataBarcode(string barcodeNo);
    Task Save(ConsumpUnscheduleSave payload, string userId);
    Task<Dictionary<string, object>> Capture(string barcodeNo);
}
