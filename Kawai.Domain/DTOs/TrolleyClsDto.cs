namespace Kawai.Domain.DTOs;

public class TrolleyClsDto: DataTableDto
{
    public string Trolley_Cls { get; set; }
     public string Description { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string Lastuser { get; set; }
    public decimal? Qty { get; set; }
    public string DDLDescription { get; set; }
}
