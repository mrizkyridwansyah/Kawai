using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IProductionNGClaimRepository
{
  
     Task<List<ProductionNGClaimDetailDto>> GetListNGDetail(RequestParameter parameter);
    Task<ProductionNGClaimDto> GetDataHeader(long claimid);
    Task Create(ProductionNGClaim prodngclaim, string userId);
    Task Update(ProductionNGClaim prodngclaim, string userId);
    Task Submit(ProductionNGClaim prodngclaim, string userId);
    Task Remove(long claimid);
    Task<List<ProductionNGClaimDto>> DDLSearch(string keyword, string status,   string userId);
    Task<List<ProductionNGClaimDto>> PickingDDLSearch(string keyword, string line, string typeDate,  bool showOptionAll, string userId);
    Task<Dictionary<string, object>> Capture(long claimid);

    






   



     
}
