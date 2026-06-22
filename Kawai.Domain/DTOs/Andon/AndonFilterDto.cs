namespace Kawai.Domain.DTOs;

public class AndonFilterDto: DataTableDto
{
	public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public string DDLDescription { get; set; }

    public string LineCode { get; set; }
    public string LineName { get; set; }

    public string ModelCls { get; set; }
    public string ModelDescs { get; set; }
}

