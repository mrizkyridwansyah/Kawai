using ClosedXML.Excel;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/qualitycheck")]
[ApiController]
public class QualityCheckController : HahaController
{
    private readonly IQualityCheckRepository _qualitycheckRepository;
    private readonly DataLogger _logger;

    public QualityCheckController(IQualityCheckRepository qualitycheckRepository, DataLogger logger)
    {
        _qualitycheckRepository = qualitycheckRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _qualitycheckRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }


    [HttpGet("detail")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _qualitycheckRepository.GetData(id);
        return Success(result);
    }

    [HttpPatch("confirm")]
    public async Task<IActionResult> Confirm([FromBody] QualityCheck model)
    {
        var before = await _qualitycheckRepository.Capture(model.Id);
        await _qualitycheckRepository.Confirm(model, Auth.User.UserID);
        var after = await _qualitycheckRepository.Capture(model.Id);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Quality Check IQC",
            EntityId = model.Id,
            ReferenceId = model.Id,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpGet("ddl-dnno-supplierdate-search")]
    public async Task<IActionResult> DDLLotSearchByStock(string keyword, string ids, string supplier, string receiptdatefrom, string receiptdateto)
    {
        var results = await _qualitycheckRepository.DDLDNNoBySupplierDateFrom(keyword, supplier, receiptdatefrom, receiptdateto);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.DN_No)).ToList();
        }

        return Success(results);
    }
    //[HttpDelete("confirm")]
    //public async Task<IActionResult> Remove(string id)
    //{
    //    var before = await _qualitycheckRepository.Capture(id);
    //    await _qualitycheckRepository.Confirm(id, Auth.User.UserID);
    //    await _logger.SaveDataLog(new DataLogDto
    //    {
    //        DocumentType = "Master Area",
    //        EntityId = id,
    //        ReferenceId = id,
    //        Action = DataLogAction.,
    //        Before = before
    //    });

    //    return Success(before);
    //}


}
