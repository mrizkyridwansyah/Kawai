using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IPartMaterialRequestOthersRepository
{
  
    Task<List<PartMaterialRequestOthersDetailDto>> GetListRequestDetail(RequestParameter parameter);
    Task<List<PartMaterialRequestOthersHistoryDto>> GetListHistory(RequestParameter parameter);
    Task<List<PartMaterialRequestOthersListScanDto>> GetListScan(RequestParameter parameter);
    Task<PartMaterialRequestOthersDto> GetDataHeader(long requestid);
    Task Create(PartMaterialRequestOthers requestothers, string userId);
    Task Update(PartMaterialRequestOthers requestothers, string userId);
     Task Remove(long requestid);
    Task<List<PartMaterialRequestOthersDto>> DDLSearch(string keyword, string status,   string userId);
    Task<Dictionary<string, object>> Capture(long requestid);
    






   



     
}
