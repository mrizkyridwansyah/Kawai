namespace Kawai.Domain.DTOs;

public class CompanyLineDto: DataTableDto
{
    public string CompanyCode { get; set; }
      public string LineCode { get; set; }
    public string LineName { get; set; }
    public string DDLDescription { get; set; }
}
