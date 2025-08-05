namespace Kawai.Domain.DTOs;

public class WarehouseDto: DataTableDto
{
    public string WarehouseCode { get; set; }
    public string WarehouseName { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string Lastuser { get; set; }
}
