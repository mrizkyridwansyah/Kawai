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
}
