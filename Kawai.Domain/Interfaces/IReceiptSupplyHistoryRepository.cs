using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IReceiptSupplyHistoryRepository
{
    Task<List<ReceiptSupplyHistoryDto>> GetList(RequestParameter param);
}
