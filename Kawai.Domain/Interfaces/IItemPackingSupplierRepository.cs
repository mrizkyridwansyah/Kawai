using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IItemPackingSupplierRepository
{
    Task<List<ItemPackingSupplierDto>> GetAll(RequestParameter param);
    Task<ItemPackingSupplierDto> GetData(string supplierCode, string itemCode);
    Task Create(ItemPackingSupplier model, string userId);
    Task Update(ItemPackingSupplier model, string userId);
    Task Remove(string supplierCode, string itemCode);
    Task<Dictionary<string, object>> Capture(string supplierCode, string itemCode);
}
