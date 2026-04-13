using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface INGClaimRepository
{
  
    Task<List<NGClaimDto>> GetList(RequestParameter parameter);
    Task<List<NGClaimDetailDto>> GetListPODetail(RequestParameter parameter);
    Task<List<NGClaimDetailDto>> GetListNGClaimDetail(RequestParameter parameter);
    Task<NGClaimDto> GetDataHeader(long claimid);
    Task Create(NGClaim ngclaim, string userId);
    Task Update(NGClaim ngclaim, string userId);
    Task Approve(NGClaim ngclaim, string userId);
    Task Remove(long claimid);
    Task PrintLabel(long claimid, string userId);
    Task<List<NGClaimReportDto>> GetListReport(string factory, long claimid);
    Task<List<NGClaimDto>> DDLSearch(string keyword,   string supplier, DateTime? periodFrom, DateTime? periodUntil, string status,   string userId);
    Task<Dictionary<string, object>> Capture(long claimid);
    






   



     
}
