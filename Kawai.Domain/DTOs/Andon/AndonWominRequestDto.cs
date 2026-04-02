namespace Kawai.Domain.DTOs;

// INI DIPAKE DI ANDON WOMIN
public class AndonWominRequestDto: DataTableDto
{
    public string RequestNo { get; set; }
    public DateTime ProductionDate { get; set; }
    public string Line { get; set; }
    public string WorkStation { get; set; }
    public string PickingArea { get; set; }
    public string Model { get; set; }
    public string PreparationStatus { get; set; }
    public string TrollyNumber { get; set; }
    public string CurrentPosition { get; set; }
    public string NextLocation { get; set; }
    public decimal TotalItem { get; set; }
    public decimal Remaining { get; set; }
    public decimal Womin { get; set; }
    public string PickingProgress { get; set; }

}

public class AndonWominRequestSummaryDto : DataTableDto
{
    public decimal TotalRequest { get;set; }
    public decimal Womin { get; set; }
    public decimal TotalItem { get;set;}
    public decimal Remaining { get; set; }
}
