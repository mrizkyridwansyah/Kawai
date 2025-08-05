using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface ILocationRepository
{
    Task<List<LocationDto>> GetAll(RequestParameter param);
    Task<List<LocationDto>> GetDDL(string keyword, string warehouseCode);
    Task<List<LocationDto>> DDLSearchByStock(string keyword, string warehouseCode, string item);
    Task<LocationDto> GetData(string locationCode);
    Task Create(Location location, string userId);
    Task Update(Location location, string userId);
    Task Remove(string locationCode, string userId);
    Task<Dictionary<string, object>> Capture(string locationCode);
    Task<List<LocationPrivilegesDto>> GetAllLocationIncludePrivileges(string userId);
}
