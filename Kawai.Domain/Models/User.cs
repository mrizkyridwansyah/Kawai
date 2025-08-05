using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Kawai.Api.Models;

public class User
{
    [Required(ErrorMessage = "UserID tidak boleh kosong")]
    [MaxLength(30, ErrorMessage = "UserID tidak boleh lebih dari 30 karakter")]
    public string UserID { get; set; }

    [Required(ErrorMessage = "FullName tidak boleh kosong")]
    [MaxLength(50, ErrorMessage = "FullName tidak boleh lebih dari 50 karakter")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Password tidak boleh kosong")]
    [MaxLength(20, ErrorMessage = "Password tidak boleh lebih dari 20 karakter")]
    public string Password { get; set; }

    [MaxLength(20, ErrorMessage = "Job Position tidak boleh lebih dari 20 karakter")]
    public string JobPositionCode { get; set; }

    [MaxLength(20, ErrorMessage = "User Group tidak boleh lebih dari 20 karakter")]
    public string UserGroupID { get; set; }

    public IFormFile ImageAttachment { get; set; }

    public string ImageName { get; set; }

    [Required(ErrorMessage = "Status Admin tidak boleh kosong")]
    public bool IsAdmin { get; set; }
}
