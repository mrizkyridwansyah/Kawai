using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IWarehouseRepository
{
    Task<List<WarehouseDto>> GetAll(RequestParameter param);

    Task<List<WarehouseDto>> GetDDL(string keyword, string factoryCode);
    Task<List<WarehouseDto>> GetDDLWarehouseLine(string keyword, string factoryCode);
    Task<List<WarehouseDto>> DDLSearchByStock(string keyword, string factoryCode, string item);

    Task<List<WarehouseDto>> GetDDLPrivileges(string keyword, string factoryCode, string userId);
    Task<List<WarehouseDto>> GetDDLPrivilegesWarehouseLine(string keyword, string factoryCode, string userId);
    Task<List<WarehouseDto>> DDLPrivilegesSearchByStock(string keyword, string factoryCode, string item, string userId);

    Task<WarehouseDto> GetData(string warehouseCode);
    Task Create(Warehouse warehouse, string userId);
    Task Update(Warehouse warehouse, string userId);
    Task Remove(string warehouseCode, string userId);
    Task<Dictionary<string, object>> Capture(string warehouseCode);
    Task<List<WarehousePrivilegesDto>> GetAllWarehouseIncludePrivileges(string userId);
}
