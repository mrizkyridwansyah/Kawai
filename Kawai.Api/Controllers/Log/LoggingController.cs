using Kawai.Domain.Interfaces.Log;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Log;

[Route("api/logging")]
[ApiController]
public class LoggingController : HahaController
{
    private readonly ILoggingRepository _loggingRepository;

    public LoggingController(ILoggingRepository loggingRepository)
    {
        _loggingRepository = loggingRepository;
    }

    [HttpPost("request/list")]
    public async Task<IActionResult> ListRequestLog([FromBody] RequestParameter parameter)
    {
        var results = await _loggingRepository.GetRequestLogAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("data/list")]
    public async Task<IActionResult> ListDataLog([FromBody] RequestParameter parameter)
    {
        var results = await _loggingRepository.GetDataLogAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("data/detail")]
    public async Task<IActionResult> GetDataLog(long id)
    {
        var result = await _loggingRepository.GetDataLogDetail(id);
        return Success(result);
    }

    [HttpPost("error/list")]
    public async Task<IActionResult> ListErrorLog([FromBody] RequestParameter parameter)
    {
        var results = await _loggingRepository.GetErrorLogAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("error/detail")]
    public async Task<IActionResult> GetErrorLog(long id)
    {
        var result = await _loggingRepository.GetErrorLogDetail(id);
        return Success(result);
    }
}
