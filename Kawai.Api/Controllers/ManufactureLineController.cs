using Kawai.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/manufactureline")]
[ApiController]
public class ManufactureLineController : HahaController
{
    private readonly IManufactureLineRepository _manufactureLineRepository;
    private readonly DataLogger _logger;

    public ManufactureLineController(IManufactureLineRepository manufactureLineRepository, DataLogger logger)
    {
        _manufactureLineRepository = manufactureLineRepository;
        _logger = logger;
    }

    

    [HttpGet("ddl-manufacture-search")]
    public async Task<IActionResult> DDLManufactureSearch(string keyword, string ids)
    {
        var results = await _manufactureLineRepository.GetManufactureDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ManufactureCode)).ToList();
        }

        return Success(results);
    }


    [HttpGet("ddl-line-search")]
    public async Task<IActionResult> DDLLineSearch(string keyword, string manufactureCode, string ids)
    {
        var results = await _manufactureLineRepository.GetLineDDL(keyword, manufactureCode);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.LineCode)).ToList();
        }

        return Success(results);
    }



}
