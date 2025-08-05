using System.ComponentModel.DataAnnotations;

namespace Kawai.Domain.Models;

public class SignIn
{
    [Required]
    public string UserName { get; set; }
    [Required]
    public string Password { get; set; }
}
