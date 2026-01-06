using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IPeriodSettingRepository
{
  
    Task<List<PeriodSettingDto>> GetListDetail(RequestParameter parameter);
    Task Update(PeriodSetting ps, string userId);
    Task<Dictionary<string, object>> Capture(int year);
    






   



     
}
