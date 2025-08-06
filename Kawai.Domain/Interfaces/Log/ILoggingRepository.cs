using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces.Log;

public interface ILoggingRepository
{
    Task<List<LoggingRequestDto>> GetRequestLogAll(RequestParameter param);
    Task<List<LoggingDataDto>> GetDataLogAll(RequestParameter param);
    Task<LoggingDataDto> GetDataLogDetail(long id);
    Task<List<LoggingErrorDto>> GetErrorLogAll(RequestParameter param);
    Task<LoggingErrorDto> GetErrorLogDetail(long id);
}
