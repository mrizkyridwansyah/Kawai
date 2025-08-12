using Kawai.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/hs")]
[ApiController]
public class HSController : HahaController
{
    private readonly IHSRepository _hsRepository;
    private readonly DataLogger _logger;

    public HSController(IHSRepository hsRepository, DataLogger logger)
    {
        _hsRepository = hsRepository;
        _logger = logger;
    }

    

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string ids)
    {
        var results = await _hsRepository.GetDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.HSCode)).ToList();
        }

        return Success(results);
    }

    
 
  
}
