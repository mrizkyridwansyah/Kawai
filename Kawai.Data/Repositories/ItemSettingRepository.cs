using Dapper;
using Kawai.Api.Models;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using System.Data;
using System.Diagnostics;

namespace Kawai.Data.Repositories;

public class ItemSettingRepository : IItemSettingRepository
{
    private readonly DbExecutor _dbExecutor;

    public ItemSettingRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ItemSettingDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_ItemSetting_List";
        return (await _dbExecutor.QueryListAsync<ItemSettingDto>(sp, param.ToQueryObject())).ToList();
    }

    

    public async Task SaveItemSetting(ItemSetting itemsettinglist, string userId)
    {
        var commands = new List<(string, object?, CommandType)>();
    

        commands.Add(("sp_WMS_ItemSetting_Delete", new
        {
            Model_Cls = itemsettinglist.Model_Cls,
            ParentItem_Code = itemsettinglist.ParentItem_Code
        }, CommandType.StoredProcedure));

        foreach (var wsSet in itemsettinglist.ItemList.Where(p => (p.AllowSetting.HasValue && p.AllowSetting.Value)))
        {
            commands.Add(("sp_WMS_ItemSetting_Upd", new
            {
                Model_Cls = itemsettinglist.Model_Cls,
                ParentItem_Code = itemsettinglist.ParentItem_Code,
                wsSet.Item_Code,
                wsSet.AllowSetting,
                wsSet.Carton_Cls, //input address untuk prod result
                wsSet.Pallet_Cls, //input address untuk prod result
                UserID = userId
            }, CommandType.StoredProcedure));
        }

        await _dbExecutor.ExecuteMultiCommandWithTransactionAsync(commands);
    }




    public async Task<Dictionary<string, object>> Capture(string model_cls ,  string parentitem_code)
    {
        string spSetting = "sp_Wms_ItemSetting_Capture";
        var wsSetting= (await _dbExecutor.QueryListAsync<dynamic>(spSetting, new { Model_Cls = model_cls, ParentItem_Code = parentitem_code })).ToList();

         
        return new Dictionary<string, object>
        {
            { "ItemSetting", wsSetting } 
        };
    }

   
}
