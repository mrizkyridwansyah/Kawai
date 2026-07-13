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
[Route("api/ngclaim")]
[ApiController]
public class NGClaimController : HahaController
{
    private readonly INGClaimRepository _ngclaimRepository;
    private readonly DataLogger _logger;

    public NGClaimController(INGClaimRepository ngclaimRepository, DataLogger logger)
    {
        _ngclaimRepository = ngclaimRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] RequestParameter parameter)
    {
        var results = await _ngclaimRepository.GetList(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("data-header")]
    public async Task<IActionResult> GetDataHeader(long claimid)
    {
        var result = await _ngclaimRepository.GetDataHeader(claimid);
        return Success(result);
    }

    //[HttpPost("list-detail")]
    //public async Task<IActionResult> GetListDetail(long claimid)
    //{
    //    var results = await _ngclaimRepository.GetListDetail(claimid);
    //    return Success(results);
    //}

    [HttpPost("list-po-detail")]
    public async Task<IActionResult> GetListPODetail([FromBody] RequestParameter parameter)
    {
        var results = await _ngclaimRepository.GetListPODetail(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("list-ngclaim-detail")]
    public async Task<IActionResult> GetListNGClaimDetail([FromBody] RequestParameter parameter)
    {
        var results = await _ngclaimRepository.GetListNGClaimDetail(parameter);
        return DataTableResult(parameter, results);
    }


    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] NGClaim model)
    {
        await _ngclaimRepository.Create(model, Auth.User.UserID);

        var after = await _ngclaimRepository.Capture(model.ClaimId.Value);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "NG Claim Material",
            EntityId = model.ClaimId.ToString(),
            ReferenceId = model.ClaimNo,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] NGClaim model)
    {
        var before = await _ngclaimRepository.Capture(model.ClaimId.Value);

        await _ngclaimRepository.Update(model, Auth.User.UserID);

        var after = await _ngclaimRepository.Capture(model.ClaimId.Value);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "NG Claim Material",
            EntityId = model.ClaimId.ToString(),
            ReferenceId = model.ClaimNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });
        return Success(after);
    }

    [HttpPatch("approve")]
    public async Task<IActionResult> Approve([FromBody] NGClaim model)
    {
        var before = await _ngclaimRepository.Capture(model.ClaimId.Value);

        await _ngclaimRepository.Approve(model, Auth.User.UserID);

        var after = await _ngclaimRepository.Capture(model.ClaimId.Value);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "NG Claim Material Approve",
            EntityId = model.ClaimId.ToString(),
            ReferenceId = model.ClaimNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });
        return Success(after);
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword,   string supplier, DateTime? periodFrom, DateTime? periodUntil, string status,   string ids)
    {
        var results = await _ngclaimRepository.DDLSearch(keyword,  supplier, periodFrom, periodUntil, status,  Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ClaimID.ToString())).ToList();
        }

        return Success(results.Take(100));
    }

    [HttpPost("report-surat-jalan")]
    public async Task<IActionResult> ExportExcel(string factory, long claimid, [FromServices] RazorViewRenderer renderer)
    {
        var results = await _ngclaimRepository.GetListReport(factory, claimid);
        if (results == null || !results.Any()) return Invalid("No Data");

        var fullHtml = await renderer.RenderAsync(
            "Templates/SuratJalan.cshtml",
            results);

        var pdfBytes = await renderer.GeneratePdfAsync(fullHtml);
        Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        return File(pdfBytes, "application/pdf", "SuratJalan_" + results[0].CustPONo);
    }


   


}
