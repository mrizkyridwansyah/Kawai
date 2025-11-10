using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IAreaRepository
{
    Task<List<AreaDto>> GetAll(RequestParameter param);
    Task<List<AreaDto>> GetDDL(string keyword, string warehouseCode);
    Task<List<AreaDto>> DDLSearchByStock(string keyword, string warehouseCode, string item);
    Task<List<AreaDto>> GetDDLPrivileges(string keyword, string warehouseCode, string userId);
    Task<List<AreaDto>> DDLPrivilegesSearchByStock(string keyword, string warehouseCode, string item, string userId);
    Task<AreaDto> GetData(string areaCode);
    Task Create(Area area, string userId);
    Task Update(Area area, string userId);
    Task Remove(string areaCode, string userId);
    Task<Dictionary<string, object>> Capture(string areaCode);
    Task<List<AreaPrivilegesDto>> GetAllAreaIncludePrivileges(string userId);
}
