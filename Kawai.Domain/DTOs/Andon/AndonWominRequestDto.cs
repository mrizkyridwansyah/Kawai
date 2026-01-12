namespace Kawai.Domain.DTOs;

// INI DIPAKE DI ANDON WOMIN
public class AndonWominRequestDto: DataTableDto
{
    public string RequestNo { get; set; }
    public DateTime ProductionDate { get; set; }
    public string Line { get; set; }
    public string WorkStation { get; set; }
    public string PickingArea { get; set; }
    public string PreparationStatus { get; set; }
    public string TrollyNumber { get; set; }
    public string CurrentPosition { get; set; }
    public string NextLocation { get; set; }
    public double TotalItem { get; set; }
    public double Remaining { get; set; }

}

public class AndonWominRequestSummaryDto : DataTableDto
{
    public double TotalRequest { get;set; }
    public double Womin { get; set; }
    public double TotalItem { get;set;}
    public double Remaining { get; set; }
}
