using Kawai.Domain.DTOs;
using Microsoft.AspNetCore.Http;

namespace Kawai.Api.DTOs;

public class UserDto : DataTableDto
{
    public string UserID { get; set; }
    public string FullName { get; set; }
    public string Password { get; set; }
    public string JobPositionCode { get; set; }
    public string UserGroupID { get; set; }
    public IFormFile ImageAttachment { get; set; }
    public string ImageName { get; set; }
    public byte[] ImageBase64 { get; set; }
    public bool IsAdmin { get; set; }
}
