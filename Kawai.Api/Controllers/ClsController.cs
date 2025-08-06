using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/cls")]
[ApiController]
public class ClsController : HahaController
{
    private readonly IClsRepository _clsRepository;
    private readonly DataLogger _logger;

    public ClsController(IClsRepository clsRepository, DataLogger logger)
    {
        _clsRepository = clsRepository;
        _logger = logger;
    }

    

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string typedata, string ids)
    {
        var results = await _clsRepository.GetDDL(keyword, typedata);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => !idList.Contains(x.ClsCode)).ToList();
        }

        return Success(results);
    }

    
 
  
}
