using ClosedXML.Excel;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/material-productionng")]
[ApiController]
public class ProductionNGClaimController : HahaController
{
    private readonly IProductionNGClaimRepository _productionngclaimRepository;
    private readonly DataLogger _logger;

    public ProductionNGClaimController(IProductionNGClaimRepository productionngclaimRepository, DataLogger logger)
    {
        _productionngclaimRepository = productionngclaimRepository;
        _logger = logger;
    }

    

    [HttpGet("data-header")]
    public async Task<IActionResult> GetDataHeader(long claimid)
    {
        var result = await _productionngclaimRepository.GetDataHeader(claimid);
        return Success(result);
    }

 

    [HttpPost("list-ng-detail")]
    public async Task<IActionResult> GetListNGDetail([FromBody] RequestParameter parameter)
    {
        var results = await _productionngclaimRepository.GetListNGDetail(parameter);
        return DataTableResult(parameter, results);
    }

   [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ProductionNGClaim model)
    {
        await _productionngclaimRepository.Create(model, Auth.User.UserID);

        var after = await _productionngclaimRepository.Capture(model.ClaimId.Value);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Production NG Claim Material",
            EntityId = model.ClaimId.ToString(),
            ReferenceId = model.ClaimNo,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] ProductionNGClaim model)
    {
        var before = await _productionngclaimRepository.Capture(model.ClaimId.Value);

        await _productionngclaimRepository.Update(model, Auth.User.UserID);

        var after = await _productionngclaimRepository.Capture(model.ClaimId.Value);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Production NG Claim Material",
            EntityId = model.ClaimId.ToString(),
            ReferenceId = model.ClaimNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });
        return Success(after);
    }

    [HttpPatch("submit")]
    public async Task<IActionResult> Submit([FromBody] ProductionNGClaim model)
    {
        var before = await _productionngclaimRepository.Capture(model.ClaimId.Value);

        await _productionngclaimRepository.Submit(model, Auth.User.UserID);

        var after = await _productionngclaimRepository.Capture(model.ClaimId.Value);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Production NG Claim Material Submit",
            EntityId = model.ClaimId.ToString(),
            ReferenceId = model.ClaimNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });
        return Success(after);
    }

    [HttpGet("picking-ddlsearch")]
    public async Task<IActionResult> PickingDDLSearch(string keyword,  string line, string typeDate,    bool showOptionAll, string ids)
    {
        var results = await _productionngclaimRepository.PickingDDLSearch(keyword, line, typeDate,  showOptionAll, Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.PickingNo)).ToList();
        }

        return Success(results);
    }


    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string status,   string ids)
    {
        var results = await _productionngclaimRepository.DDLSearch(keyword, status,  Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ClaimID.ToString())).ToList();
        }

        return Success(results.Take(100));
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(long claimid)
    {
        var before = await _productionngclaimRepository.Capture(claimid);

        await _productionngclaimRepository.Remove(claimid);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Delete Production NG Claim Material ",
            EntityId = claimid.ToString(),
            ReferenceId = claimid.ToString(),
            Before = before,
            After = null,
            Action = DataLogAction.Delete,
            Activity = "Delete Production NG Claim Material "
        });

        return Success();
    }



}
