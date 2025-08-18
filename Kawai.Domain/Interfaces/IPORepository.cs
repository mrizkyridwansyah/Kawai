using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IPORepository
{
    Task<List<PODto>> GetList(RequestParameter parameter);
    Task<PODto> GetDetail(string poNumber);
    Task<List<PODetailDto>> GetListDetail(RequestParameter parameter);
}
