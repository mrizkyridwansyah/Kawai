using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class Location
{
    [Required(ErrorMessage = "Warehouse tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Warehouse tidak boleh lebih dari 25 karakter")]
    public string WarehouseCode { get; set; }

    [Required(ErrorMessage = "Location Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Location Code tidak boleh lebih dari 25 karakter")]
    public string LocationCode { get; set; }

    [Required(ErrorMessage = "Location Name tidak boleh kosong")]
    [MaxLength(100, ErrorMessage = "Location Name tidak boleh lebih dari 100 karakter")]
    public string LocationName { get; set; }
}
