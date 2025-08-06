namespace Kawai.Domain.DTOs;

public class NGDto: DataTableDto
{
    public string NGCode { get; set; }
    public string Description { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string Lastuser { get; set; }
    public bool IsCommon { get; set; }
}
