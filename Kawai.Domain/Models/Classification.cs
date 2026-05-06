using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class Classification
{
    [Required(ErrorMessage = "Code tidak boleh kosong")]
    public string Code { get; set; }

    [Required(ErrorMessage = "Description tidak boleh kosong")]
    [MaxLength(150, ErrorMessage = "Description tidak boleh lebih dari 150 karakter")]
    public string Description { get; set; }

    public string TableName { get; set; }

 
}
