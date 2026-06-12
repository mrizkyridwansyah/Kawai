using ClosedXML.Excel;
using Kawai.Api.Services;
using Hangfire;
using Kawai.Data.Repositories;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/reprint")]
[ApiController]
public class ReprintController : HahaController
{
    private readonly IReprintRepository _reprintRepository;
    private readonly DataLogger _logger;

    public ReprintController(IReprintRepository reprintRepository, DataLogger logger)
    {
        _reprintRepository = reprintRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _reprintRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("printupdate")]
    public async Task<IActionResult> Printupdate([FromBody] List<Dictionary<string, object>> rows)
    {
        if (!rows.Any()) return NoContent();

        foreach (var rowMap in rows)
        {
            string key = rowMap.TryGetValue("Key", out var keyVal) ? keyVal?.ToString() ?? "" : "";
            string value = rowMap.TryGetValue("Value", out var val) ? val?.ToString() ?? "" : "";

            if (string.IsNullOrWhiteSpace(key))
                continue;

            await _reprintRepository.UpdatePrintValue(key, value, Auth.User.UserID);

        }



        return Success();
    }

    [HttpPost("printpdf")]
    public async Task<IActionResult> LabelBarcode([FromBody] List<SelectedPrintDto> selectedPrint)
    {
        try
        {
            if (selectedPrint == null || !selectedPrint.Any())
                return BadRequest("No barcode selected");

            string key = "ReprintBarcode_" + Guid.NewGuid().ToString();
            BackgroundJob.Enqueue<ExportService>(service => service.ExportPdfReprintBarcode(selectedPrint, Auth.User.UserID, key));

            return Pending(message: "Data Export PDF sedang diproses!");
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

}
