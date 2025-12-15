using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class WorkStation
{
    [Required]
    [MaxLength(15)]
    public string WorkStationCode { get; set; }

    [Required]
    [MaxLength(100)]
    public string WorkStationName { get; set; }
 
}
