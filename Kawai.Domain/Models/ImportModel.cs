using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class ImportModel
{
    [Required]
    public string TemplateName { get; set; }

    [Required]
    [AllowedValues(["TEST", "EXECUTE"])]
    public string Action { get; set; }

    [Required]
    public IFormFile File { get; set; }
}

public class ImportHistory
{
    [Key]
    [MaxLength(100)]
    public string Id { get; set; }

    [MaxLength(100)]
    public string Key { get; set; }

    [MaxLength(100)]
    public string Template { get; set; }

    [AllowedValues(["SUCCESS", "FAILED"])]
    public string Status { get; set; }

    [MaxLength(255)]
    public string FileName { get; set; }

    [MaxLength(255)]
    public string ContentType { get; set; }
    public long ProcessDuration { get; set; }
    public long SizeFile { get; set; }
    public int RowsCount { get; set; }
    public int ValidRowsCount { get; set; }
    public int InvalidRowsCount { get; set; }
    public string UserId { get; set; }

}