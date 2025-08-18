namespace Kawai.Domain.DTOs;

public class ItemPackingSupplierDto: DataTableDto
{
    public string SupplierCode { get; set; }
    public string SupplierName { get; set; }
    public string ItemCode { get; set; }
    public string ItemName { get; set; }
    public string UnitClsCode { get; set; }
    public string UnitClsName { get; set; }
    public double QtyPacking { get; set; }
    public DateTime LastUpdate { get; set; }
    public string LastUser { get; set; }
}
