using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class WorkStationSetting
{


    public string LineCode { get; set; }
    public List<WorkStationSettingList> SettingList { get; set; }


}
public class WorkStationSettingList
{
    public string WorkStationCode { get; set; }
    public bool? AllowSetting { get; set; } = false;
    
}
