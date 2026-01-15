using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IPhysicalInventoryRepository
{
    Task<List<PhysicalInventoryDto>> GetList(RequestParameter parameter);
    Task Update(List<PhysicalInventoryUpdateDto> model, string userId);
    Task<Dictionary<string, object>> Capture(List<PhysicalInventoryUpdateDto> model);
}
