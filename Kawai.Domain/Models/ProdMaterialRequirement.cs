using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class ProdMaterialRequirement
{
    public string ParamKey { get; set; }

    [Required(ErrorMessage = "Factory Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Factory Code tidak boleh lebih dari 25 karakter")]
    public string Factory { get; set; }

    [Required(ErrorMessage = "Process Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Process Code tidak boleh lebih dari 25 karakter")]
    public string Process { get; set; }

    [Required(ErrorMessage = "Line Code tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Line Code tidak boleh lebih dari 25 karakter")]
    public string Line { get; set; }

    [Required(ErrorMessage = "Model tidak boleh kosong")]
    [MaxLength(25, ErrorMessage = "Model tidak boleh lebih dari 25 karakter")]
    public string Model { get; set; }

    [Required(ErrorMessage = "Schedule (Up To) tidak boleh kosong")]
    public DateTime? ScheduleDateTo { get; set; }
}
