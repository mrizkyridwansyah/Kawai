using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IAndonWominRequestRepository
{

    #region ANDON
    Task<List<AndonWominRequestDto>> GetListNSummary(string line, string Area);
    Task<List<AndonWominRequestDetailDto>> GetListWomin(RequestParameter parameter);
    Task<List<AndonWominRequestDetailDto>> GetListWominByLine(RequestParameter parameter);

    #endregion

}
