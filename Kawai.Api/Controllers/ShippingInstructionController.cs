using Kawai.Api.Services;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/shipping-instruction")]
[ApiController]
public class ShippingInstructionController : HahaController
{
    private readonly IShippingInstructionRepository _shippingInstructionRepository;

    public ShippingInstructionController(IShippingInstructionRepository shippingInstructionRepository)
    {
        _shippingInstructionRepository = shippingInstructionRepository;
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(
        [FromQuery] string keyword,
        [FromQuery] string custCode,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] string ids)
    {
        var results = await _shippingInstructionRepository.GetFilterDDL(custCode, dateFrom, dateTo);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var key = keyword.Trim();
            results = results
                .Where(x => !string.IsNullOrEmpty(x.SI_No) && x.SI_No.Contains(key, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.SI_No)).ToList();
        }

        return Success(results);
    }

    [HttpGet("list")]
    public async Task<IActionResult> GetList(
        [FromQuery] string poNo,
        [FromQuery] bool isNew = true)
    {
        if (string.IsNullOrWhiteSpace(poNo))
            return Invalid("Order Number harus diisi.");

        var results = await _shippingInstructionRepository.GetList(poNo, isNew);
        return Success(results);
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit([FromBody] List<ShippingInstructionRequest> requests)
    {
        if (requests == null || requests.Count == 0)
            return Invalid("Pilih minimal 1 data untuk disubmit.");

        await _shippingInstructionRepository.Submit(requests, Auth.User.UserID);
        return Success(message: "Submit Shipping Instruction berhasil.");
    }

    [HttpPost("picking")]
    public async Task<IActionResult> UpdatePicking([FromBody] List<ShippingPickingRequest> requests)
    {
        if (requests == null || requests.Count == 0)
            return Invalid("Pilih minimal 1 serial untuk dipicking.");

        await _shippingInstructionRepository.UpdatePicking(requests, Auth.User.UserID);
        return Success(message: "Picking serial berhasil.");
    }

    [HttpPost("report-surat-jalan")]
    public async Task<IActionResult> ExportExcel(string sino, [FromServices] RazorViewRenderer renderer)
    {
        var results = await _shippingInstructionRepository.GetListReport(sino);
        if (results == null || !results.Any()) return Invalid("No Data");

        var fullHtml = await renderer.RenderAsync(
            "Templates/SuratJalan.cshtml",
            results);

        var pdfBytes = await renderer.GeneratePdfAsync(fullHtml);
        Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        return File(pdfBytes, "application/pdf", "SuratJalan_" + results[0].CustPONo);
    }

}
