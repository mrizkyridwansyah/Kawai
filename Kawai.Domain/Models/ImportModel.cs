using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class ImportModel
{
    [Required]
    [AllowedValues(["TEST", "EXECUTE"])]
    public string Action { get; set; }

    [Required]
    public IFormFile File { get; set; }
}
