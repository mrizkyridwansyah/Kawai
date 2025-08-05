namespace Kawai.Domain.DTOs;

public class MenuDto
{
    public string MenuID { get; set; }
    public string MenuName { get; set; }
    public string MenuDescription { get; set; }
    public string MenuGroup { get; set; }
    public string SubGroup { get; set; }
    public int GroupIndex { get; set; }
    public int MenuIndex { get; set; }
    public int? SubGroupIndex { get; set; }
    public string ImageName { get; set; }
    public bool AllowAccess { get; set; }
    public bool AllowUpdate { get; set; }
    public bool AllowPrice { get; set; }
}
