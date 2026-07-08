namespace Kawai.Domain.DTOs;

public class ItemSettingDto: DataTableDto
{
    public string Model_Cls { get; set; }
    public string ParentItem_Code { get; set; }
    public string Item_Code { get; set; }
    public string Item_Name { get; set; } = "";
    public string? Carton_Cls { get; set; } 
    public string? Pallet_Cls { get; set; } 
    public string DDLDescription { get; set; }
    public bool AllowSetting { get; set; }
    public DateTime? RegisterDate { get; set; }
    public string RegisterUser { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
   
}
