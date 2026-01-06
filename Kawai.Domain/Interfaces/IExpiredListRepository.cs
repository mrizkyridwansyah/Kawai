using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IExpiredListRepository
{
    Task<List<ExpiredListDto>> GetList(RequestParameter parameter);
}