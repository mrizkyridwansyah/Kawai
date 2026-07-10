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
[Route("api/manufactureline")]
[ApiController]
public class ManufactureLineController : HahaController
{
    private readonly IManufactureLineRepository _manufactureLineRepository;
    private readonly DataLogger _logger;

    public ManufactureLineController(IManufactureLineRepository manufactureLineRepository, DataLogger logger)
    {
        _manufactureLineRepository = manufactureLineRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _manufactureLineRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _manufactureLineRepository.GetData(id);
        return Success(result);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] Manufactureline model)
    {
        var before = await _manufactureLineRepository.Capture(model.LineCode);
        await _manufactureLineRepository.Update(model, Auth.User.UserID);
        var after = await _manufactureLineRepository.Capture(model.LineCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master ManufactureLine",
            EntityId = model.LineCode,
            ReferenceId = model.LineCode,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }


    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestParameter parameter)
    {
        var results = await _manufactureLineRepository.GetAll(parameter);
        if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["Manufacture Code", "Line Code", "Line Name", "IP Printer"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.ManufactureCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LineCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LineName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.IPPrinter); 
        }

        ExcelHelper.AutofitColumns(ws, 1, headers.Count);

        var range = ws.Range(1, 1, rowIdx, headers.Count);
        ExcelHelper.SetBorders(range);

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        var fileBytes = ms.ToArray();
        var base64File = Convert.ToBase64String(fileBytes);

        return Success(base64File);
    }


    [HttpGet("ddl-manufacture-search")]
    public async Task<IActionResult> DDLManufactureSearch(string keyword, string ids)
    {
        var results = await _manufactureLineRepository.GetManufactureDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ManufactureCode)).ToList();
        }

        return Success(results);
    }


    [HttpGet("ddl-line-search")]
    public async Task<IActionResult> DDLLineSearch(string keyword, string manufactureCode, string ids)
    {
        var results = await _manufactureLineRepository.GetLineDDL(keyword, manufactureCode);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.LineCode)).ToList();
        }

        return Success(results);
    }



}
