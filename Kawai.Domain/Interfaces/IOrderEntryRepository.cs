using Kawai.Domain.DTOs;

namespace Kawai.Domain.Interfaces;

public interface IOrderEntryRepository
{
    Task<List<OrderEntryFilterDto>> GetFilterDDL(string custCode, DateTime? dateFrom, DateTime? dateTo);
}
