using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class BOMWorkStation
{


    public string ParentItem_Code { get; set; }
    public string WorkStationCode { get; set; }
    public List<BOMWorkStationList> BomSetting { get; set; }


}
public class BOMWorkStationList
{
    public string ChildItem_Code { get; set; }
    public double? Qty { get; set; }
    public bool? AllowSetting { get; set; } = false;
    
}
