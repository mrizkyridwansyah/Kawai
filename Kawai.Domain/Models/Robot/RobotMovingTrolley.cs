using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models.Robot;

public class RobotMovingTrolley
{
    [Required(ErrorMessage = "Kode Trolley harus diisi")]
    [MaxLength(20, ErrorMessage = "Kode Trolley tidak boleh melebihi 20 karakter")]
    public string TrolleyCode { get; set; }

    [Required(ErrorMessage = "Kode Area harus diisi")]
    [MaxLength(25, ErrorMessage = "Kode Area tidak boleh melebihi 25 karakter")]
    public string AreaCode { get; set; }
}
