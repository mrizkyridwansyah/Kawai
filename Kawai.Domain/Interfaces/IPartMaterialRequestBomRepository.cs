using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IPartMaterialRequestBomRepository
{
    Task<List<PartMaterialRequestBomDto>> GetListHeader(RequestParameter parameter);
    Task<List<PartMaterialRequestBomDetilDto>> GetListDetail(List<PartMaterialRequestBomModel> parameter);
    Task<List<StockDto>> GetListStock(RequestParameter parameter);
    Task Save(List<PartMaterialRequestBomModel> model, string userId);
    Task Remove(long requestId, string userId);
    Task<Dictionary<string, object>> Capture(long productionId);
    Task<Dictionary<string, object>> CaptureRequest(long requestId);
}
