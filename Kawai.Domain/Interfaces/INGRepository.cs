using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface INGRepository
{
    Task<List<NGDto>> GetAll(RequestParameter param);
    Task<List<NGDto>> GetDDL(string keyword);
    Task<NGDto> GetData(string NGCode);
    Task Create(NG ng, string userId);
    Task Update(NG ng, string userId);
    Task Remove(string ngCode, string userId);
    Task<Dictionary<string, object>> Capture(string ngCode);


}
