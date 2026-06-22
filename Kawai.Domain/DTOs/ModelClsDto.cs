using Kawai.Domain.DTOs;
using Microsoft.AspNetCore.Http;

namespace Kawai.Api.DTOs;

public class ModelClsDto : DataTableDto
{
    public string Model_Cls { get; set; }
    public string Description { get; set; }
    public int CycleTime { get; set; }
    public IFormFile ImageAttachment { get; set; }
    public string ImageName { get; set; }
    public byte[] ImageBase64 { get; set; }
    public string RegisterUser { get; set; }
    public DateTime RegisterDate { get; set; }
    public string LastUser { get; set; }
    public DateTime? LastUpdate { get; set; }
}
