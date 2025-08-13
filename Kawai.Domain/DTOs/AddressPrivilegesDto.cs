namespace Kawai.Domain.DTOs;

public class AddressPrivilegesDto
{
    public string WarehouseCode { get; set; }
    public string AreaCode { get; set; }
    public string AddressCode { get; set; }
    public string AddressName { get; set; }
    public bool AllowAccess { get; set; }
}
