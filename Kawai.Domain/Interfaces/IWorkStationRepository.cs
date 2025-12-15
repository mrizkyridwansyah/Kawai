using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IWorkStationRepository
{
    Task<List<WorkStationDto>> GetAll(RequestParameter param);
    Task<List<WorkStationDto>> GetDDL(string keyword);
    Task<WorkStationDto> GetData(string WorkStationCode);
    Task Create(WorkStation ws, string userId);
    Task Update(WorkStation ws, string userId);
    Task Remove(string workstationCode, string userId);
    Task<Dictionary<string, object>> Capture(string workstationCode);


}
