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
[Route("api/shipping-instruction")]
[ApiController]
public class ShippingInstructionController : HahaController
{
    private readonly IShippingInstructionRepository _shippingInstructionRepository;
    private readonly DataLogger _logger;


    public ShippingInstructionController(IShippingInstructionRepository shippingInstructionRepository, DataLogger logger)
     {
        _shippingInstructionRepository = shippingInstructionRepository;
        _logger = logger;
     
    }

    [HttpGet("po-ddlsearch")]
    public async Task<IActionResult> PODDLSearch(string keyword, string custCode, string typeDate, DateTime? periodFrom, DateTime? periodUntil, bool showOptionAll, string ids)
    {
        var results = await _shippingInstructionRepository.GetPODDL(keyword, custCode, typeDate, periodFrom, periodUntil, showOptionAll, Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.PONumber)).ToList();
        }

        return Success(results.Take(100));
    }

    [HttpGet("si-ddlsearch")]
    public async Task<IActionResult> SIDDLSearch(string keyword, string supplier, DateTime? periodFrom, DateTime? periodUntil , string sourceMenu, string orderEntry, string ids)
    {
        var results = await _shippingInstructionRepository.GetSIDDL(keyword, supplier, periodFrom, periodUntil, sourceMenu, orderEntry, Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ShippingInstructionNo.ToString())).ToList();
        }

        return Success(results.Take(100));
    }

    [HttpPost("list-picking")]
    public async Task<IActionResult> ListPicking([FromBody] RequestParameter parameter)
    {
        var results = await _shippingInstructionRepository.GetListPicking(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("data-header")]
    public async Task<IActionResult> GetDataHeader(string shippinginstructionno)
    {
        var result = await _shippingInstructionRepository.GetDataHeader(shippinginstructionno);
        return Success(result);
    }

    [HttpPost("list-detail")]
    public async Task<IActionResult> GetListDetail([FromBody] RequestParameter parameter)
    {
        var results = await _shippingInstructionRepository.GetListDetail(parameter);
        return DataTableResult(parameter, results);
    }


    [HttpPost("save-picking")]
    public async Task<IActionResult> Save(ShippingInstructionPicking model)
    {
        var header = model.Header.FirstOrDefault();
        var before = await _shippingInstructionRepository.Capture(header.ShippingInstructionNo);
        await _shippingInstructionRepository.SavePicking(model, Auth.User.UserID);
        var after = await _shippingInstructionRepository.Capture(header.ShippingInstructionNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Shipping Instruction Picking",
            EntityId = header.ShippingInstructionNo,
            ReferenceId = header.PONumber,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ShippingInstruction model)
    {
        await _shippingInstructionRepository.Create(model, Auth.User.UserID);

        var after = await _shippingInstructionRepository.Capture(model.ShippingInstructionNo.ToString());
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Shipping Instruction",
            EntityId = model.ShippingInstructionNo.ToString(),
            ReferenceId = model.ShippingInstructionNo,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] ShippingInstruction model)
    {
        var before = await _shippingInstructionRepository.Capture(model.ShippingInstructionNo.ToString());

        await _shippingInstructionRepository.Update(model, Auth.User.UserID);

        var after = await _shippingInstructionRepository.Capture(model.ShippingInstructionNo);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Shipping Instruction",
            EntityId = model.ShippingInstructionNo.ToString(),
            ReferenceId = model.ShippingInstructionNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });
        return Success(after);
    }

 
    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string shippinginstructionno)
    {
        var before = await _shippingInstructionRepository.Capture(shippinginstructionno);

        await _shippingInstructionRepository.Remove(shippinginstructionno);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Delete Shipping Instruction ",
            EntityId = shippinginstructionno.ToString(),
            ReferenceId = shippinginstructionno.ToString(),
            Before = before,
            After = null,
            Action = DataLogAction.Delete,
            Activity = "Delete Shipping Instruction "
        });

        return Success();
    }

 
    [HttpPost("report-surat-jalan")]
    public async Task<IActionResult> ExportExcel(string sino, [FromServices] RazorViewRenderer renderer)
    {
        var results = await _shippingInstructionRepository.GetListReport(sino);
        if (results == null || !results.Any()) return Invalid("No Data");

        var fullHtml = await renderer.RenderAsync(
            "Templates/SuratJalan_SI.cshtml",
            results);

        var pdfBytes = await renderer.GeneratePdfAsync(fullHtml);
        Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        return File(pdfBytes, "application/pdf", "SuratJalan_" + results[0].CustPONo);
    }

}
