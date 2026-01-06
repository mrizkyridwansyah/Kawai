using Kawai.Domain.Shared;
using Kawai.Domain.DTOs;

namespace Kawai.Domain.Interfaces;

public interface ISlowMovingReportRepository
{
    Task<List<SlowMovingReportDto>> GetAll(RequestParameter param);
}