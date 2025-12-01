using Kawai.Data.Repositories;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/po")]
[ApiController]
public class POController : HahaController
{
    private readonly IPORepository _poRepository;
    private readonly DataLogger _logger;

    public POController(IPORepository poRepository, DataLogger logger)
    {
        _poRepository = poRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] RequestParameter parameter)
    {
        var results = await _poRepository.GetList(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetDetail(string dnNumber)
    {
        var result = await _poRepository.GetDetail(dnNumber);
        return Success(result);
    }

    [HttpPost("list-detail")]
    public async Task<IActionResult> GetListDetail([FromBody] RequestParameter parameter)
    {
        var results = await _poRepository.GetListDetail(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string factory, string supplier, string typeDate, DateTime? periodFrom, DateTime? periodUntil, bool showOptionAll, string ids)
    {
        var results = await _poRepository.GetDDL(keyword, factory, supplier, typeDate, periodFrom, periodUntil, showOptionAll, Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.PONumber)).ToList();
        }

        return Success(results);
    }
}
