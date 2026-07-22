using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IPartMaterialRequestWominRepository
{
    Task<List<PartMaterialRequestWominDto>> GetListHeader(RequestParameter parameter);
    Task<List<PartMaterialRequestWominDetilDto>> GetListDetail(List<PartMaterialRequestWominModel> parameter);
    Task<List<StockDto>> GetListStock(RequestParameter parameter);
    Task<List<StockScanDto>> GetListScan(RequestParameter parameter);
    Task<Dictionary<string, object>> Save(List<PartMaterialRequestWominModel> model, string userId);
    Task Remove(long requestId, string userId);
    Task CheckValidGenerateDetail(List<PartMaterialRequestWominModel> models);

    Task<PartMaterialRequestWominEditDto> GetDataRequirement(long IDSeq);
    Task UpdateReq(PartMaterialRequestWominEditModel editwomin, string userId);
    
    Task<List<PartMaterialRequestWominReportDto>> WominReport(long requestId);
    Task<Dictionary<string, object>> CaptureRequirement(long IDSeq);
    Task<Dictionary<string, object>> CaptureRequest(long requestId);
}
