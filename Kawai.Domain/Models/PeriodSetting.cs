using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class PeriodSetting
{
    public int Year { get; set; }
    public List<PeriodSettingDetail> Details { get; set; } = new List<PeriodSettingDetail>();
}

public class PeriodSettingDetail
{
        public string Period { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthName { get; set; }
        public string? StartPeriod { get; set; }
        public string? EndPeriod { get; set; }
        public string? StartSO { get; set; }
        public string? FinishSO { get; set; }
     



}


