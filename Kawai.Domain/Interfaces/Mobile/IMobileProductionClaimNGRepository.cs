using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;

namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileProductionClaimNGRepository
{
    Task<List<ProductionClaimNGClaimNoDto>> GetClaimNoDDL(string keyword, string linecode , string claimno);
    Task<ProductionClaimNGDto> GetDataBarcode(string barcodeNo, string linecode, string claimno);
    Task<List<ProductionClaimNGDto>> GetListDetailMaterial(string claimno, string linecode);
    Task<List<ManufactureLineDto>> GetLineDDL(string keyword,string warehousecode);
    Task<List<ManufactureLineDto>> GetProcessDDL(string keyword);
    Task<List<ProductionClaimNGDetailDto>> GetListDetail(string linecode, string claimno, string pickingno, string itemCode);
    Task<bool> Save(MobilProductionClaimNGSubmit payload, string userId);
    Task<Dictionary<string, object>> Capture(string barcodeNo, string claimno, string liencode);

 
}
