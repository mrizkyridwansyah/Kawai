using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IAreaRepository
{
    Task<List<AreaDto>> GetAll(RequestParameter param);
    Task<List<AreaDto>> GetDDL(string keyword);
    Task<List<AreaDto>> DDLSearchByStock(string keyword, string item);
    Task<AreaDto> GetData(string areaCode);
    Task Create(Area location, string userId);
    Task Update(Area location, string userId);
    Task Remove(string areaCode, string userId);
    Task<Dictionary<string, object>> Capture(string areaCode);
    Task<List<AreaPrivilegesDto>> GetAllAreaIncludePrivileges(string userId);
}
