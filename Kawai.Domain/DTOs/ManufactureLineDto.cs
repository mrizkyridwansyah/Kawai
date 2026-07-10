namespace Kawai.Domain.DTOs;

public class ManufactureLineDto: DataTableDto
{
    public string ManufactureCode { get; set; }
    public string ManufactureName { get; set; }
    public string LineCode { get; set; }
    public string LineName { get; set; }
    public string IPPrinter { get; set; }
    public string DDLDescription { get; set; }

    public string FactoryCode { get; set; }
    public string FactoryName { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string Lastuser { get; set; }

}
