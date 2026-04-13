using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IStopPointRepository
{
    Task<List<StopPointDto>> GetAll(RequestParameter param);
    Task<List<StopPointDto>> GetDDL(string keyword);
    Task<List<StopPointDto>> GetDDLByAddress(string keyword ,string line, string workstation);
    Task<StopPointDto> GetData(string StopPointCode);
    Task Create(StopPoint stoppoint, string userId);
    Task Update(StopPoint stoppoint, string userId);
    Task SaveStopPointAddress(string stopPoint, string addressKey);
    Task Remove(string stoppointCode, string userId);
    Task<Dictionary<string, object>> Capture(string stoppointCode);


}
