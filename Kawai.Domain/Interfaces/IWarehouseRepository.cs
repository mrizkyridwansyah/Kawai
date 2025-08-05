using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IWarehouseRepository
{
    Task<List<WarehouseDto>> GetAll(RequestParameter param);
    Task<List<WarehouseDto>> GetDDL(string keyword);
    Task<List<WarehouseDto>> DDLSearchByStock(string keyword, string item);
    Task<WarehouseDto> GetData(string warehouseCode);
    Task Create(Warehouse warehouse, string userId);
    Task Update(Warehouse warehouse, string userId);
    Task Remove(string warehouseCode, string userId);
    Task<Dictionary<string, object>> Capture(string warehouseCode);
    Task<List<WarehousePrivilegesDto>> GetAllWarehouseIncludePrivileges(string userId);
}
