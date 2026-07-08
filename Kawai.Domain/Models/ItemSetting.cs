using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class ItemSetting
{


    public string Model_Cls { get; set; }
    public string ParentItem_Code { get; set; }
    public List<ItemSettingList> ItemList { get; set; }


}
public class ItemSettingList
{
    public string Item_Code { get; set; }
    public bool? AllowSetting { get; set; } = false;
    public string? Carton_Cls { get; set; }
    public string? Pallet_Cls { get; set; }


}
