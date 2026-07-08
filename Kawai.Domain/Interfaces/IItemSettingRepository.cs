using Kawai.Api.Models;
using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IItemSettingRepository
{
    Task<List<ItemSettingDto>> GetAll(RequestParameter param);
    Task SaveItemSetting(ItemSetting itemlist,string userId);
    Task<Dictionary<string, object>> Capture(string Model_Cls, string ParentItem_Code);


}
