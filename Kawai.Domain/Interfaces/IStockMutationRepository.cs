using Kawai.Domain.DTOs;

namespace Kawai.Domain.Interfaces;

public interface IStockMutationRepository
{
    Task<List<StockMutationDto>> GetPendingStock();
    Task Receipt(string id);
    Task Production(string id);
    Task Adjustment(string id);
    Task Transfer(string id);
    Task PrepareConsume(string id);
    //Task UsedConsume(string id);
    Task Split(string id);
}
