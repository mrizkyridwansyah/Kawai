using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class DeliveryPlace
{
    [Required(ErrorMessage = "Trade Code tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Trade Code tidak boleh lebih dari 15 karakter")]
    public string Trade_Code { get; set; }

    [Required(ErrorMessage = "Location Code tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Location Code tidak boleh lebih dari 15 karakter")]
    public string Location_Code { get; set; }

    [Required(ErrorMessage = "Location Name tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Location Name tidak boleh lebih dari 25 karakter")]
    public string Location_Name { get; set; }
}
