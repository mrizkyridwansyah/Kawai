namespace Kawai.Domain.DTOs;

public class ManufactureLineDto: DataTableDto
{
    public string ManufactureCode { get; set; }
    public string ManufactureName { get; set; }
    public string LineCode { get; set; }
    public string LineName { get; set; }
    public string DDLDescription { get; set; }
}
