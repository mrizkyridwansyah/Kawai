using Kawai.Api.Models;
using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IWorkStationSettingRepository
{
    Task<List<WorkStationSettingDto>> GetAll(RequestParameter param);
    Task<List<CompanyLineDto>> GetLineCompanyDDL(string keyword, string companyCode, string manufacture);

    Task SaveWorkStationSetting(WorkStationSetting settinglist,string userId);
    Task<Dictionary<string, object>> Capture(string LineCode);


}
