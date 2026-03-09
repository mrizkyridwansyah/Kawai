using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class TrolleyCls
{
    [Required(ErrorMessage = "Trolley Cls tidak boleh kosong")]
    [MaxLength(2, ErrorMessage = "Trolley Cls tidak boleh lebih dari 2 karakter")]
    public string Trolley_Cls { get; set; }

    [Required(ErrorMessage = "Description tidak boleh kosong")]
    [MaxLength(150, ErrorMessage = "Description tidak boleh lebih dari 150 karakter")]
    public string Description { get; set; }

    public decimal? Qty { get; set; }

   
}
