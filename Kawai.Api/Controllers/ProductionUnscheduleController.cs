using DocumentFormat.OpenXml.Office2010.Excel;
using Kawai.Domain;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/production-unschedule")]
[ApiController]
public class ProductionUnscheduleController : HahaController
{
    private readonly IProductionUnscheduleRepository _repo;
    private readonly DataLogger _logger;

    public ProductionUnscheduleController(IProductionUnscheduleRepository repo, DataLogger logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpGet("ddl-line-search")]
    public async Task<IActionResult> DDLLineSearch(string keyword, string ids)
    {
        var results = await _repo.GetLineUnscheduleDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.LineCode ?? "")).ToList();
        }
        return Success(results.Take(100));
    }

    [HttpGet("ddl-parentitem-search")]
    public async Task<IActionResult> DDLParentItem(string keyword, string ids)
    {
        var results = await _repo.GetParentItemUnscheduleDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ParentItemCode??"")).ToList();
        }
        return Success(results.Take(100));
        
    }

    [HttpPost("list-bom")]
    public async Task<IActionResult> ListHeader([FromBody] RequestParameter parameter)
    {
        var results = await _repo.GetBOMRequirement(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("list-result")]
    public async Task<IActionResult> ListDetail([FromBody] RequestParameter parameter)
    {
        var results = await _repo.GetListResults(parameter);
        return DataTableResult(parameter,results);
    }

    [HttpGet("list-detail")]
    public async Task<IActionResult> ResultDetail(string id)
    {
        var results = await _repo.GetListResultsDetail(id);
        return Success(results);
    }


    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] ProductionUnscheduleModel payload)
    {
        var logs = new List<DataLogDto>();

        if (payload == null)
            return Invalid("Invalid Request Data");                                                        

        //foreach (var item in models)
        //{
        //    //var before = await _repo.Capture(item.ProductionId);
        //    logs.Add(new DataLogDto
        //    {
        //        DocumentType = "Production Result Manual Input",
        //        EntityId = item.ProductionId.ToString(),
        //        ReferenceId = item.ProductionId.ToString(),
        //        //Before = before,
        //        After = null,
        //        Action = DataLogAction.Update,
        //        Activity = "Save Production Result Manual Input"
        //    });
        //}

        await _repo.Save(payload, Auth.User.UserID);

        //foreach (var log in logs)
        //{
        //    var after = await _productionResultManualInputRepository.Capture(log.EntityId.ToInt32());
        //    log.After = after;

        //    await _logger.SaveDataLog(log);
        //}

        return Success();
    }

    

   
     }
