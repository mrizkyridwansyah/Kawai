using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface ITrolleyRepository
{
    Task<List<TrolleyDto>> GetAll(RequestParameter param);
    Task<List<TrolleyDto>> GetDDL(string keyword);
    Task<TrolleyDto> GetData(string TrolleyCode);
    Task Create(Trolley trolley, string userId);
    Task Update(Trolley trolley, string userId);
    Task Remove(string trolleyCode, string userId);
    Task<Dictionary<string, object>> Capture(string trolleyCode);


}
