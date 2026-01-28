namespace Kawai.Domain.DTOs;

public class AreaDto: DataTableDto
{
    public string WarehouseCode { get; set; }   
    public string WarehouseName { get; set; }
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public string ItemType { get; set; }
    public string ItemTypeDesc { get; set; }
    public int? PickingSequence { get; set; }
    public DateTime RegisterDate { get; set; }
    public string RegisterUser { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string LastUser { get; set; }
    public string DDLDescription { get; set; }
}
