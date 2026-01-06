namespace Kawai.Domain.DTOs;

public class SlowMovingReportDto : DataTableDto
{
    // kolom tetap
    public string LongStock { get; set; }
    public string WHCode { get; set; }
    public string WHName { get; set; }
    public string Item_Code { get; set; }
    public string Item_Name { get; set; }
    public string Unit_Name { get; set; }
    public string Remarks { get; set; }

    // kolom dynamic pivot
    public Dictionary<string, decimal?> PeriodQty { get; set; } = new();
}
