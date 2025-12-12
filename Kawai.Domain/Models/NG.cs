using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class NG
{
    [Required(ErrorMessage = "NG Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "NG Code tidak boleh lebih dari 25 karakter")]
    public string NGCode { get; set; }

    [Required(ErrorMessage = "NG Description tidak boleh kosong")]
    [MaxLength(100, ErrorMessage = "NG Code tidak boleh lebih dari 100 karakter")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Flag Common tidak boleh kosong")]
    public bool? IsCommon { get; set; }
}
