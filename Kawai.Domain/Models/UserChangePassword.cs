using Kawai.Domain.Shared.Validator;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class UserChangePassword
{
    [Required(ErrorMessage = "Password Lama tidak boleh kosong")]
    [MaxLength(20, ErrorMessage = "Password Lama tidak boleh lebih dari 20 karakter")]
    [Display(Name = "Password Lama")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Password Baru tidak boleh kosong")]
    [MaxLength(20, ErrorMessage = "Password Baru tidak boleh lebih dari 20 karakter")]
    [Display(Name = "Password Baru")]
    public string NewPassword { get; set; }

    [Required(ErrorMessage = "Konfirmasi Password tidak boleh kosong")]
    [MaxLength(20, ErrorMessage = "Konfirmasi Password tidak boleh lebih dari 20 karakter")]
    [SameAs(nameof(NewPassword))]
    [Display(Name = "Konfirmasi Password")]
    public string ConfirmPassword { get; set; }
}
