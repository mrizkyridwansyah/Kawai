using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class AddressRepository : IAddressRepository
{
    private readonly DbExecutor _dbExecutor;

    public AddressRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<AddressDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_Address_List";
        return (await _dbExecutor.QueryListAsync<AddressDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<AddressDto> GetData(string areaCode)
    {
        string sp = "sp_Wms_Address_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<AddressDto>(sp, new { AddressCode = areaCode });
    }

    public async Task<List<AddressDto>> GetDDL(string keyword, string warehouse, string area)
    {
        string sp = "sp_Wms_Address_DDL";
        return (await _dbExecutor.QueryListAsync<AddressDto>(sp, new { Keyword = keyword ?? "", WarehouseCode = warehouse, AreaCode = area })).ToList();
    }

    public async Task<List<AddressDto>> DDLSearchByStock(string keyword, string warehouse, string area, string item)
    {
        string sp = "sp_Wms_Address_DDLByStock";
        return (await _dbExecutor.QueryListAsync<AddressDto>(sp, new { Keyword = keyword ?? "", WarehouseCode = warehouse, AreaCode = area, ItemCode = String.IsNullOrEmpty(item) ? "ALL" : item })).ToList();
    }

    public async Task<List<AddressDto>> GetDDLPrivileges(string keyword, string warehouse, string area, string userId)
    {
        string sp = "sp_Wms_AddressPrivileges_DDL";
        return (await _dbExecutor.QueryListAsync<AddressDto>(sp, new { Keyword = keyword ?? "", WarehouseCode = warehouse, AreaCode = area, UserId = userId })).ToList();
    }

    public async Task<List<AddressDto>> DDLPrivilegesSearchByStock(string keyword, string warehouse, string area, string item, string userId)
    {
        string sp = "sp_Wms_AddressPrivileges_DDLByStock";
        return (await _dbExecutor.QueryListAsync<AddressDto>(sp, new { Keyword = keyword ?? "", WarehouseCode = warehouse, AreaCode = area, ItemCode = String.IsNullOrEmpty(item) ? "ALL" : item, UserId = userId })).ToList();
    }

    public async Task Create(Address address, string userId)
    {
        address.AddressCode = await _dbExecutor.QuerySingleOrDefaultAsync<string>("sp_Wms_Address_GenerateCode", new { address.WarehouseCode, address.AreaCode });
        string sql = @"sp_Wms_Address_Create";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            address.WarehouseCode,
            address.AreaCode,
            address.AddressCode,
            address.AddressName,
            RegisterBy = userId
        });
    }

    public async Task Update(Address address, string userId)
    {
        string sql = @"sp_Wms_Address_Update";
        await _dbExecutor.ExecuteAsync(sql, new
        {
            address.WarehouseCode,
            address.AreaCode,
            address.AddressCode,
            address.AddressName,
            UpdateBy = userId
        });
    }

    public async Task Remove(string addressCode, string userId)
    {
        string sql = "sp_Wms_Address_Delete";
        await _dbExecutor.ExecuteAsync(sql, new { AddressCode = addressCode });
    }

    public async Task<Dictionary<string, object>> Capture(string addressCode)
    {
        string sp = "sp_Wms_Address_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { AddressCode = addressCode });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

    public async Task<List<AddressPrivilegesDto>> GetAllAddressIncludePrivileges(string userId)
    {
        string sp = "sp_WMS_UserSetup_UserPrivilegeAddress";
        return (await _dbExecutor.QueryListAsync<AddressPrivilegesDto>(sp, new { UserID = userId })).ToList();
    }
}
