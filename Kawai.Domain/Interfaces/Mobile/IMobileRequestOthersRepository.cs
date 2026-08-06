using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileRequestOthersRepository
{
    Task<List<RequestOthersRequestNoDto>> GetRequestNoDDL(string keyword, string linecode , string requestno);
    Task<RequestOthersDto> GetDataBarcode(string barcodeNo, string linecode, string requestno);
    Task<List<RequestOthersDto>> GetListDetailMaterial(string requestno, string linecode);
    Task<List<ManufactureLineDto>> GetLineDDL(string keyword,string warehousecode);
    Task<List<ManufactureLineDto>> GetProcessDDL(string keyword);
    Task<List<RequestOthersDetailDto>> GetListDetail(string linecode, string requestno, string itemCode);
    Task<bool> Save(MobileRequestOthersSubmit payload, string userId);
    Task<Dictionary<string, object>> Capture(string barcodeNo, string requestno, string liencode);

 
}
