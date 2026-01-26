using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class Trolley
{
    [Required(ErrorMessage = "Trolley Code tidak boleh kosong")]
    [MaxLength(20, ErrorMessage = "Trolley Code tidak boleh lebih dari 20 karakter")]
    public string TrolleyCode { get; set; }

    [Required(ErrorMessage = "Description tidak boleh kosong")]
    [MaxLength(150, ErrorMessage = "Description tidak boleh lebih dari 150 karakter")]
    public string Description { get; set; }

    public string Trolley_Cls { get; set; }

    [Required(ErrorMessage = "Status Active tidak boleh kosong")]
    public bool? IsActive { get; set; }
}
