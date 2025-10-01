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
    public async Task<List<MenuMobileDto>> GetAllMenuMobileIncludePrivileges(string userId)
    {
        string sp = "sp_WMS_UserSetup_UserMobilePrivilege";
        return (await _dbExecutor.QueryListAsync<MenuMobileDto>(sp, new { UserID = userId })).ToList();
    }
    public async Task<List<MenuDto>> GetUserMenuPrivileges(string userId)
    {
        string sp = "sp_Wms_Login_GetUserMenu";
        return (await _dbExecutor.QueryListAsync<MenuDto>(sp, new { UserId = userId })).ToList();
    }
    public async Task<List<MenuMobileDto>> GetUserMenuMobilePrivileges(string userId)
    {
        string sp = "sp_Wms_Login_GetUserMenuMobile";
        return (await _dbExecutor.QueryListAsync<MenuMobileDto>(sp, new { UserId = userId })).ToList();
    }
    public async Task SavePrivileges(string userId, Privileges privileges)
    {
        var commands = new List<(string, object?, CommandType)>();

        commands.Add(("sp_WMS_UserSetup_UserPrivilegeDelete", new
        {
            UserID = privileges.UserId
        }, CommandType.StoredProcedure));

        foreach (var menuPriv in privileges.MenuPrivileges.Where(p => (p.AllowAccess.HasValue && p.AllowAccess.Value) || (p.AllowUpdate.HasValue && p.AllowUpdate.Value) || (p.AllowPrice.HasValue && p.AllowPrice.Value)))
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

        foreach (var menuPriv in privileges.MenuMobilePrivileges.Where(p => p.AllowAccess.HasValue && p.AllowAccess.Value))
        {
            commands.Add(("sp_WMS_UserSetup_UserPrivilegeMobileUpd", new
            {
                UserID = privileges.UserId,
                menuPriv.MenuID,
                menuPriv.AllowAccess,
            }, CommandType.StoredProcedure));
        }

        foreach (var warehousePriv in privileges.WarehousePrivileges.Where(p => p.AllowAccess.HasValue && p.AllowAccess.Value))
        {
            commands.Add(("sp_WMS_UserSetup_UserPrivilegeWarehouseUpd", new
            {
                UserID = privileges.UserId,
                warehousePriv.WarehouseCode,
                warehousePriv.AllowAccess,
                UpdateBy = userId
            }, CommandType.StoredProcedure));
        }

        await _dbExecutor.ExecuteMultiCommandWithTransactionAsync(commands);
    }

    public async Task<Dictionary<string, object>> Capture(string userId)
    {
        string spMenuPriv = "sp_Wms_Privileges_CaptureMenuPrivileges";
        var menuPrivileges = (await _dbExecutor.QueryListAsync<dynamic>(spMenuPriv, new { UserID = userId })).ToList();

        string spMobilePriv = "sp_Wms_Privileges_CaptureMenuMobilePrivileges";
        var mobilePrivileges = (await _dbExecutor.QueryListAsync<dynamic>(spMobilePriv, new { UserID = userId })).ToList();

        string spWHPriv = "sp_Wms_Privileges_CaptureWarehousePrivileges";
        var warehousePrivileges = (await _dbExecutor.QueryListAsync<dynamic>(spWHPriv, new { UserID = userId })).ToList();

        return new Dictionary<string, object>
        {
            { "MenuPrivileges", menuPrivileges },
            { "MobilePrivileges", mobilePrivileges },
            { "WarehousePrivileges", warehousePrivileges }
        };
    }
}
