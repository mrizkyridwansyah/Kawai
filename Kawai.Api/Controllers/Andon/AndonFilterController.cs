using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Andon;

[Route("api/andon/filter")]
[ApiController]
public class AndonFilterController : HahaController
{
    private readonly IAndonFilterRepository _andonFilter;

    public AndonFilterController(IAndonFilterRepository repo)
    {
        _andonFilter = repo;
    }

    //get ddl area
    [HttpGet("ddlarea")]
    public async Task<IActionResult> DDLArea(string keyword, string ids, string warehouseCode)
    {
        var results = await _andonFilter.DDLArea(keyword, warehouseCode);

        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.AreaCode)).ToList();
        }
        return Success(results);
    }



}
