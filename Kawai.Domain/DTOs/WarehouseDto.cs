using Kawai.Domain.Shared;

namespace Kawai.Domain.DTOs;

public class WarehouseDto: DataTableDto
{
    public string WarehouseCode { get; set; }
    public string WarehouseName { get; set; }
    public string AdmGroup { get; set; }
    public string AdmGroupName { get; set; }
    public string StockControlCls { get; set; }
    public string NGCls { get; set; }
    public DateTime UseEndDate { get; set; }
    public DateTime? LastUpdate { get; set; }
    public string Lastuser { get; set; }
}
