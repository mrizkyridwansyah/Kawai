namespace Kawai.Domain.DTOs;

public class WarehousePrivilegesDto
{
    public string FactoryCode { get; set; }
    public string FactoryName { get; set; }
    public string WarehouseCode { get; set; }
    public string WarehouseName { get; set; }
    public bool AllowAccess { get; set; }
}
