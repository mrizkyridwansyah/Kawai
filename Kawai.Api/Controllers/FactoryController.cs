using Kawai.Domain;
using Kawai.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/factory")]
[ApiController]
public class FactoryController : HahaController
{
    private readonly IFactoryRepository _factoryRepository;
    private readonly DataLogger _logger;

    public FactoryController(IFactoryRepository factoryRepository, DataLogger logger)
    {
        _factoryRepository = factoryRepository;
        _logger = logger;
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string ids)
    {
        var results = await _factoryRepository.GetDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.CompanyCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("ddlsearch-privileges")]
    public async Task<IActionResult> DDLPrivilegesSearch(string keyword, string ids)
    {
        var results = await _factoryRepository.GetDDLPrivileges(keyword, Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.CompanyCode)).ToList();
        }

        return Success(results);
    }

}
