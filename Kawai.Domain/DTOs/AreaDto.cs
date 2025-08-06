namespace Kawai.Domain.DTOs;

public class AreaDto: DataTableDto
{
    public string WarehouseCode { get; set; }   
    public string WarehouseName { get; set; }
    public string LocationCode { get; set; }
    public string LocationName { get; set; }
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public DateTime RegisterDate { get; set; }
    public string RegisterUser { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
}
