namespace Kawai.Domain.DTOs;

public class WorkStationDto: DataTableDto
{
    public string WorkStationCode { get; set; }
    public string WorkStationName { get; set; }
    public string DDLDescription { get; set; }
    public DateTime? RegisterDate { get; set; }
    public string RegisterUser { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
   
}
