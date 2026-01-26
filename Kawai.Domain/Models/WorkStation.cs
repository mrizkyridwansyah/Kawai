using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class WorkStation
{
   
    [Required(ErrorMessage = "Work Station Code tidak boleh kosong")]
    [MaxLength(15, ErrorMessage = "Work Station Code tidak boleh lebih dari 15 karakter")]
    public string WorkStationCode { get; set; }

    [Required(ErrorMessage = "Work Station Name tidak boleh kosong")]
    [MaxLength(100, ErrorMessage = "Work Station Name tidak boleh lebih dari 100 karakter")]
    public string WorkStationName { get; set; }
 
}
