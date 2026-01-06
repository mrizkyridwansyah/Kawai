using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IPartMaterialRequestWominRepository
{
    Task<List<PartMaterialRequestWominDto>> GetListHeader(RequestParameter parameter);
    Task<List<PartMaterialRequestWominDetilDto>> GetListDetail(List<PartMaterialRequestWominModel> parameter);
    Task<List<StockDto>> GetListStock(RequestParameter parameter);
    Task Save(List<PartMaterialRequestWominModel> model, string userId);
    Task Remove(long requestId, string userId);
    Task<Dictionary<string, object>> Capture(long productionId);
    Task<Dictionary<string, object>> CaptureRequest(long requestId);
}
