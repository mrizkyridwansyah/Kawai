using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class Warehouse
{
    [Required]
    [MaxLength(25)]
    public string WarehouseCode { get; set; }

    [Required]
    [MaxLength(100)]
    public string WarehouseName { get; set; }

    [Required]
    [MaxLength(15)]
    public string AdmGroup { get; set; }

    [Required]
    [MaxLength(2)]
    [AllowedValues(["01", "02"])]
    public string StockControlCls { get; set; }

    [Required]
    [MaxLength(2)]
    [AllowedValues(["01", "02"])]
    public string NGCls { get; set; }

    public DateTime? UseEndDate { get; set; }
}
