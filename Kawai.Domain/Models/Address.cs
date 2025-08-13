using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class Address
{
    [Required(ErrorMessage = "Warehouse tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Warehouse tidak boleh lebih dari 25 karakter")]
    public string WarehouseCode { get; set; }

    [Required(ErrorMessage = "Area tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Area tidak boleh lebih dari 25 karakter")]
    public string AreaCode { get; set; }

    [Required(ErrorMessage = "Address Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Address Code tidak boleh lebih dari 25 karakter")]
    public string AddressCode { get; set; }

    [Required(ErrorMessage = "Address Name tidak boleh kosong")]
    [MaxLength(100, ErrorMessage = "Address Name tidak boleh lebih dari 100 karakter")]
    public string AddressName { get; set; }
}
