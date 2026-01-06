namespace Kawai.Domain.DTOs;

// INI DIPAKE DI WEB & MOBILE
public class PeriodSettingDto: DataTableDto
{
    public string Period { get; set; }
    public string Year { get; set; }
    public string Month { get; set; }
    public string MonthName { get; set; }
    public string StartDate { get; set; }
    public string StartTime { get; set; }
    public string EndDate { get; set; }
    public string EndTime { get; set; }
    public string StartSODate { get; set; }
    public string StartSOTime { get; set; }
    public string EndSODate { get; set; }
    public string EndSOTime { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
    public DateTime? RegisterDate { get; set; }
    public string RegisterUser { get; set; }
}



 