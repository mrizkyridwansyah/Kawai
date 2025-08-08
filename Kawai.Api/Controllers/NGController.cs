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
[Route("api/ng")]
[ApiController]
public class NGController : HahaController
{
    private readonly INGRepository _ngRepository;
    private readonly DataLogger _logger;

    public NGController(INGRepository ngRepository, DataLogger logger)
    {
        _ngRepository = ngRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _ngRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string ids)
    {
        var results = await _ngRepository.GetDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.NGCode)).ToList();
        }

        return Success(results);
    }

    

    [HttpGet("detail")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _ngRepository.GetData(id);
        return Success(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] NG model)
    {
        await _ngRepository.Create(model, Auth.User.UserID);

        var after = await _ngRepository.Capture(model.NGCode);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master NG",
            EntityId = model.NGCode,
            ReferenceId = model.Description,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] NG model)
    {
        var before = await _ngRepository.Capture(model.NGCode);
        await _ngRepository.Update(model, Auth.User.UserID);
        var after = await _ngRepository.Capture(model.NGCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master NG",
            EntityId = model.NGCode,
            ReferenceId = model.NGCode,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string id)
    {
        var before = await _ngRepository.Capture(id);
        await _ngRepository.Remove(id, Auth.User.UserID);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master NG",
            EntityId = id,
            ReferenceId = id,
            Action = DataLogAction.Delete,
            Before = before
        });

        return Success(before);
    }

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestParameter parameter)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["NG Code", "Description", "Common", "Last Update", "Last User"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        var results = await _ngRepository.GetAll(parameter);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.NGCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Description);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.IsCommon);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LastUpdate.HasValue ? result.LastUpdate.Value.ToString("dd MMM yyyy HH:mm") : "");
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Lastuser);
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

  
}
