using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class Area
{
    [Required(ErrorMessage = "Warehouse tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Warehouse tidak boleh lebih dari 25 karakter")]
    public string WarehouseCode { get; set; }

    [Required(ErrorMessage = "Location tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Location tidak boleh lebih dari 25 karakter")]
    public string LocationCode { get; set; }

    [Required(ErrorMessage = "Area Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Area Code tidak boleh lebih dari 25 karakter")]
    public string AreaCode { get; set; }

    [Required(ErrorMessage = "Area Name tidak boleh kosong")]
    [MaxLength(100, ErrorMessage = "Area Name tidak boleh lebih dari 100 karakter")]
    public string AreaName { get; set; }
}
