using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class NG
{
    [Required]
    [MaxLength(25)]
    public string NGCode { get; set; }

    [Required]
    [MaxLength(100)]
    public string Description { get; set; }

    public bool IsCommon { get; set; }
}
