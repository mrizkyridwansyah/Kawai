using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Andon;

[Route("api/andon/womin-request")]
[ApiController]
public class AndonWominRequestController : HahaController
{
    private readonly IAndonWominRequestRepository _andonWominRequest;

    public AndonWominRequestController(IAndonWominRequestRepository repo)
    {
        _andonWominRequest = repo;
    }


    [HttpGet("list")]
    public async Task<IActionResult> List(string line, string area)
    {
        var results = await _andonWominRequest.GetListNSummary(line, area);
        return Success(results);
    }

    [HttpPost("list-womindetail")]
    public async Task<IActionResult> ListStock([FromBody] RequestParameter parameter)
    {
        var results = await _andonWominRequest.GetListWomin(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("list-womindetailbyline")]
    public async Task<IActionResult> ListStockbyline([FromBody] RequestParameter parameter)
    {
        var results = await _andonWominRequest.GetListWominByLine(parameter);
        return DataTableResult(parameter, results);
    }
}
