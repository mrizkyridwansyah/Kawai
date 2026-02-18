namespace Kawai.Domain.DTOs;

public class StopPointDto: DataTableDto
{
    public string StopPointCode { get; set; }
    public string Description { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string Lastuser { get; set; }
    public decimal? PickingSeq { get; set; }
    public bool IsActive { get; set; }
    public string DDLDescription { get; set; }
}
