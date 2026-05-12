using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IReprintRepository
{
    Task<List<ReprintDto>> GetAll(RequestParameter param);
    Task UpdatePrintValue(string keyData, string valueData, string userId);
    Task<List<LabelBarcodeDetailDto>> GetListBarcodeDetail(List<string> barcodeNos);


}
