namespace Kawai.Domain.DTOs;

public class AreaPrivilegesDto
{
    public string FactoryCode { get; set; }
    public string WarehouseCode { get; set; }
    public string WarehouseName { get; set; }
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public bool AllowAccess { get; set; }
}
