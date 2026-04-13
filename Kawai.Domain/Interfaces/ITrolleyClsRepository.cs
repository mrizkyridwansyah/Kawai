using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface ITrolleyClsRepository
{
    Task<List<TrolleyClsDto>> GetAll(RequestParameter param);
    Task<TrolleyClsDto> GetData(string Trolley_Cls);
    Task Create(TrolleyCls trolley, string userId);
    Task Update(TrolleyCls trolley, string userId);
    Task Remove(string trolley_Cls, string userId);
    Task<Dictionary<string, object>> Capture(string trolley_Cls);


}
