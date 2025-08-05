using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using System.Data;

namespace Kawai.Data.Repositories;

public class MenuRepository : IMenuRepository
{
    private readonly DbExecutor _dbExecutor;

    public MenuRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<MenuDto>> GetAllMenuIncludePrivileges(string userId)
    {
        string sp = "sp_WMS_UserSetup_UserPrivilege";
        return (await _dbExecutor.QueryListAsync<MenuDto>(sp, new { UserID = userId })).ToList();
    }
    public async Task<List<MenuDto>> GetUserMenuPrivileges(string userId)
    {
        string sp = "sp_Wms_Login_GetUserMenu";
        return (await _dbExecutor.QueryListAsync<MenuDto>(sp, new { UserId = userId })).ToList();
    }
    public async Task SavePrivileges(string userId, Privileges privileges)
    {
        var commands = new List<(string, object?, CommandType)>();

        commands.Add(("sp_WMS_UserSetup_UserPrivilegeDelete", new
        {
            UserID = privileges.UserId
        }, CommandType.StoredProcedure));

        foreach (var menuPriv in privileges.MenuPrivileges)
        {
            commands.Add(("sp_WMS_UserSetup_UserPrivilegeUpd", new
            {
                UserID = privileges.UserId,
                menuPriv.MenuID,
                menuPriv.AllowAccess,
                menuPriv.AllowUpdate,
                menuPriv.AllowPrice
            }, CommandType.StoredProcedure));
        }

        foreach (var warehousePriv in privileges.WarehousePrivileges)
        {
            commands.Add(("sp_WMS_UserSetup_UserPrivilegeWarehouseUpd", new
            {
                UserID = privileges.UserId,
                warehousePriv.WarehouseCode,
                warehousePriv.AllowAccess,
                UpdateBy = userId
            }, CommandType.StoredProcedure));
        }

        await _dbExecutor.ExecuteTransactionAsync(commands);
    }

    public async Task<Dictionary<string, object>> Capture(string userId)
    {
        string sp = "sp_Wms_Privileges_CaptureAll";
        var result = await _dbExecutor.QueryListAsync<dynamic>(sp, new { UserID = userId });

        if (result == null)
            return new Dictionary<string, object>();

        var list = new List<Dictionary<string, object>>();
        foreach (var row in result)
        {
            var dict = ((IDictionary<string, object>)row)
                .ToDictionary(k => k.Key, v => v.Value);
            list.Add(dict);
        }

        return new Dictionary<string, object>
        {
            { "AllPrivileges", list }
        };
    }
}
