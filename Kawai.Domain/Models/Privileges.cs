using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class Privileges
{
    [Required]
    public string UserId { get; set; }

    [Required]
    public List<MenuPrivilege> MenuPrivileges { get; set; }

    [Required]
    public List<MenuMobilePrivilege> MenuMobilePrivileges { get; set; }

    [Required]
    public List<FactoryPrivilege> FactoryPrivileges { get; set; }

    [Required]
    public List<WarehousePrivilege> WarehousePrivileges { get; set; }

    [Required]
    public List<AreaPrivilege> AreaPrivileges { get; set; }

    [Required]
    public List<GroupingClassPrivilege> GroupingClassPrivileges { get; set; }
}

public class MenuPrivilege
{
    public string MenuID { get; set; }
    public bool? AllowAccess { get; set; } = false;
    public bool? AllowUpdate { get; set; } = false;
    public bool? AllowPrice { get; set; } = false;
}

public class MenuMobilePrivilege
{
    public string MenuID { get; set; }
    public bool? AllowAccess { get; set; } = false;
}

public class GroupingClassPrivilege
{
    public string GroupingClassPartCode { get; set; }
    public bool? AllowAccess { get; set; } = false;
}

public class FactoryPrivilege
{
    public string FactoryCode { get; set; }
    public bool? AllowAccess { get; set; } = false;
}

public class WarehousePrivilege
{
    public string FactoryCode { get; set; }
    public string WarehouseCode { get; set; }
    public bool? AllowAccess { get; set; } = false;
}

public class AreaPrivilege
{
    public string WarehouseCode { get; set; }
    public string AreaCode { get; set; }
    public bool? AllowAccess { get; set; } = false;
}
