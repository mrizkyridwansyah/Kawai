using Kawai.Api.Models;
using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IBOMWorkStationRepository
{
    Task<List<BOMWorkStationDto>> GetAll(RequestParameter param);
    Task<List<BOMWorkStationHeaderDto>> GetBOMWorkStationHeader(string linecode, string parentitem_code, string workstationcode);
    Task<List<BOMWorkStationDetailDto>> GetBOMWorkStationDetail(string linecode, string parentitem_code, string workstationcode);
    Task<List<BOMWorkStationDto>> GetModelClsDDL(string keyword);
    Task<List<BOMWorkStationDto>> GetItemByModelClsDDL(string keyword, string modelCls);
    Task SaveBOMWorkStation(BOMWorkStation bomsetting,string userId);
    Task<Dictionary<string, object>> Capture(string ParentItem_Code, string WorkStationCode);


}
