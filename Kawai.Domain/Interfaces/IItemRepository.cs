using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IItemRepository
{
    Task<List<ItemDto>> GetAll(RequestParameter param);
    Task<List<ItemDto>> GetDDL(string keyword);
    Task<List<WarehouseDto>> GetWarehouseDDL(string keyword);
    Task<ItemDto> GetData(string itemCode);
    Task Create(Item item, string userId);
    Task Update(Item item, string userId);
    Task Remove(string itemCode);
    Task<Dictionary<string, object>> Capture(string itemCode);
}
