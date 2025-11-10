using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IPORepository
{
    Task<List<PODto>> GetList(RequestParameter parameter);
    Task<PODto> GetDetail(string poNumber);
    Task<List<PODetailDto>> GetListDetail(RequestParameter parameter);
    Task<List<PODto>> GetDDL(string keyword, string supplier, string typeDate, DateTime? periodFrom, DateTime? periodUntil, bool showOptionAll);

}
