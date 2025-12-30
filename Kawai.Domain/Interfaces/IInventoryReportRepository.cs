using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IInventoryReportRepository
{
    Task<List<InventoryReportDto>> GetAll(RequestParameter param);
}
