using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IPickingListReportRepository
{
    Task<List<PickingListReportDto>> GetAll(RequestParameter param);
}
