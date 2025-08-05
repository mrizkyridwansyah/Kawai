namespace Kawai.Domain.DTOs;

public class LocationPrivilegesDto
{
    public string WarehouseCode { get; set; }
    public string LocationCode { get; set; }
    public string LocationName { get; set; }
    public bool AllowAccess { get; set; }
}
