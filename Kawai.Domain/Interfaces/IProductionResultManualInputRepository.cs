using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IProductionResultManualInputRepository
{
    Task<List<ProductionResultManualInputDto>> GetListHeader(RequestParameter parameter);
    Task<List<ProductionResultManualInputDetailDto>> GetListDetail(List<ProductionResultManualInputModel> parameter);
    Task<List<StockDto>> GetListStock(RequestParameter parameter);
    Task Save(List<ProductionResultManualInputModel> model, string userId);
    Task<Dictionary<string, object>> Capture(long productionId);
    
}
