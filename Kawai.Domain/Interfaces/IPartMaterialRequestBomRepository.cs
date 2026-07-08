using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IPartMaterialRequestBomRepository
{
    Task<List<PartMaterialRequestBomDto>> GetListHeader(RequestParameter parameter);
    Task<List<PartMaterialRequestBomDetilDto>> GetListDetail(List<PartMaterialRequestBomModel> parameter);
    Task<List<StockDto>> GetListStock(RequestParameter parameter);
    Task<List<NGClaimReportDto>> GetListReport(string requestno);
    Task<PartMaterialRequestBomHeaderDto> GetDataHeader(long requestId, string itemCode);
    Task Save(PartMaterialRequestBomHeaderModel model, string userId);
    Task Update(PartMaterialRequestBomHeaderModel model, string userId);
    Task Remove(long requestId, string userId);

    Task<List<StockScanDto>> GetListScan(RequestParameter parameter);
    Task<PartMaterialRequestBomDetilItemDto> GetData(long idSeq);
    Task UpdateReq(PartMaterialRequestBomDetailModel payload, string userId);

    Task<Dictionary<string, object>> Capture(string poNumber);
    Task<Dictionary<string, object>> CaptureRequest(long requestId);
    Task<Dictionary<string, object>> CaptureRequirement(long idSeq);
}
