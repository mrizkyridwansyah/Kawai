namespace Kawai.Domain.DTOs;

public class AddressDto: DataTableDto
{
    public string WarehouseCode { get; set; }   
    public string WarehouseName { get; set; }
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public string AddressCode { get; set; }
    public string AddressName { get; set; }
    public DateTime RegisterDate { get; set; }
    public string RegisterUser { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }

    public string DDLDescription { get; set; }
}
