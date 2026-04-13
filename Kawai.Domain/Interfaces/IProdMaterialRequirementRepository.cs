using Kawai.Domain.DTOs;
using Kawai.Domain.Models;

namespace Kawai.Domain.Interfaces;

public interface IProdMaterialRequirementRepository
{
    Task<ProdMaterialRequirementDto> GetLastCalculation(string factory);
    Task<List<ProdMaterialRequirementDetailDto>> GetListDetail(string factory);
    Task Save(ProdMaterialRequirement model, string userId);
    Task<Dictionary<string, object>> Capture(string paramKey);
}
