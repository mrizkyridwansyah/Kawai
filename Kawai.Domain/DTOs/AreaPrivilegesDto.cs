namespace Kawai.Domain.DTOs;

public class AreaPrivilegesDto
{
    public string WarehouseCode { get; set; }
    public string LocationCode { get; set; }
    public string AreaCode { get; set; }
    public string AreaName { get; set; }
    public bool AllowAccess { get; set; }
}
