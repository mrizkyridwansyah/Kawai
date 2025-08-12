using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IDeliveryPlaceRepository
{
    Task<List<DeliveryPlaceDto>> GetAll(RequestParameter param);
    Task<List<DeliveryPlaceDto>> GetDDL(string trade_code, string keyword);
    Task<DeliveryPlaceDto> GetData(string trade_code, string location_code);
    Task Create(DeliveryPlace dp,  string userId);
    Task Update(DeliveryPlace dp,  string userId);
    Task Remove(string trade_code, string location_code, string userId);
    Task<Dictionary<string, object>> Capture(string trade_code, string location_code);


}
