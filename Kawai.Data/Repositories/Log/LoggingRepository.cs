using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Log;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories.Log;

public class LoggingRepository : ILoggingRepository
{
    private readonly LogExecutor _logExecutor;

    public LoggingRepository(LogExecutor logExecutor)
    {
        _logExecutor = logExecutor;
    }

    public async Task<List<LoggingRequestDto>> GetRequestLogAll(RequestParameter param)
    {
        string sp = "sp_Logging_RequestLogList";
        return (await _logExecutor.QueryListAsync<LoggingRequestDto>(sp, param.ToQueryObject())).ToList();
    }
    public async Task<List<LoggingDataDto>> GetDataLogAll(RequestParameter param)
    {
        string sp = "sp_Logging_DataLogList";
        return (await _logExecutor.QueryListAsync<LoggingDataDto>(sp, param.ToQueryObject())).ToList();
    }
    public async Task<LoggingDataDto> GetDataLogDetail(long id)
    {
        string sp = "sp_Logging_DataLogDetail";
        return await _logExecutor.QueryFirstOrDefaultAsync<LoggingDataDto>(sp, new { Id = id });
    }
    public async Task<List<LoggingErrorDto>> GetErrorLogAll(RequestParameter param)
    {
        string sp = "sp_Logging_ErrorLogList";
        return (await _logExecutor.QueryListAsync<LoggingErrorDto>(sp, param.ToQueryObject())).ToList();
    }
    public async Task<LoggingErrorDto> GetErrorLogDetail(long id)
    {
        string sp = "sp_Logging_ErrorLogDetail";
        return await _logExecutor.QueryFirstOrDefaultAsync<LoggingErrorDto>(sp, new { Id = id });
    }

}
