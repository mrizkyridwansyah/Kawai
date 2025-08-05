using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class Privileges
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public List<MenuPrivilege> MenuPrivileges { get; set; }

    [Required]
    public List<WarehousePrivilege> WarehousePrivileges { get; set; }
}

public class MenuPrivilege
{
    public string MenuID { get; set; }
    public bool? AllowAccess { get; set; } = false;
    public bool? AllowUpdate { get; set; } = false;
    public bool? AllowPrice { get; set; } = false;
}

public class WarehousePrivilege
{
    public string WarehouseCode { get; set; }
    public bool? AllowAccess { get; set; } = false;
}
