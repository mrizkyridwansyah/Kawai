using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IProductionQualityJudgementRepository
{
    Task<List<ProductionQualityJudgementDto>> GetListHeader(RequestParameter parameter);
    //Task<List<ProductionQualityJudgementDetailDto>> GetListDetail(List<ProductionQualityJudgementModel> parameter);
    Task Save(List<ProductionQualityJudgementModel> model, string userId);
    Task<Dictionary<string, object>> Capture(long productionId);

}
