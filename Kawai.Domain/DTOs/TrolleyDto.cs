namespace Kawai.Domain.DTOs;

public class TrolleyDto: DataTableDto
{
    public string TrolleyCode { get; set; }
    public string Trolley_Cls { get; set; }
    public string Trolley_ClsDescs { get; set; }
    public string Description { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string Lastuser { get; set; }
    public bool IsActive { get; set; }
    public string DDLDescription { get; set; }
}
