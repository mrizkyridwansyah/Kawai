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
    public string StopPointCode { get; set; } = "";
    public string StopPointCode2 { get; set; } = "";
    public string StopPointCode3 { get; set; } = "";


}
