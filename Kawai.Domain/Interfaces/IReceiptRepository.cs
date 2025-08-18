using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IReceiptRepository
{
    Task<List<ReceiptDto>> GetList(RequestParameter parameter);
    Task<ReceiptDto> GetDetail(long id);
    Task<List<ReceiptDetailDto>> GetListDetail(long id);
    Task Create(Receipt receipt, string userId);
    Task CreateUsingMutation(Receipt receipt, string userId);
    Task Update(Receipt receipt, string userId);
    Task Remove(long id);
    Task<Dictionary<string, object>> Capture(long id);
}
