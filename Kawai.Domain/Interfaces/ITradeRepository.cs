using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface ITradeRepository
{
    Task<List<TradeDto>> GetAll(RequestParameter param);
    Task<List<ListDeliveryPlaceDto>> GetDeliveryList(string parentitem_code);
    Task<List<TradeDto>> GetDDL(string keyword);
    Task<List<TradeDto>> GetDDLCustomer();
    Task<TradeDto> GetData(string Trade_Code);
    Task SaveTradeDelivery(TradeSaveDelivery tradedeliverylist, string userId);
    Task Create(Trade trade, string userId);
    Task Update(Trade trade, string userId);
    Task Remove(string trade_Code, string userId);
    Task<Dictionary<string, object>> Capture(string trade_Code);


}
