using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/stoppoint")]
[ApiController]
public class StopPointController : HahaController
{
    private readonly IStopPointRepository _stoppointRepository;
    private readonly DataLogger _logger;

    public StopPointController(IStopPointRepository stoppointRepository, DataLogger logger)
    {
        _stoppointRepository = stoppointRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _stoppointRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }


    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string ids)
    {
        var results = await _stoppointRepository.GetDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.StopPointCode)).ToList();
        }

        return Success(results);
    }


    [HttpGet("detail")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _stoppointRepository.GetData(id);
        return Success(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] StopPoint model)
    {
        await _stoppointRepository.Create(model, Auth.User.UserID);

        var after = await _stoppointRepository.Capture(model.StopPointCode);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Stop Point",
            EntityId = model.StopPointCode,
            ReferenceId = model.Description,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPost("settingdata")]
    public async Task<IActionResult> SettingData([FromBody] StopPointSetting model)
    {
        if (model == null || model.Address == null || !model.Address.Any())
            return BadRequest("No address selected");

        foreach (var addr in model.Address)
        {
            // contoh simpan ke DB
            await _stoppointRepository.SaveStopPointAddress(
                model.StopPoint,
                addr.Key
            );
        }

        return Success();
    }


    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] StopPoint model)
    {
        var before = await _stoppointRepository.Capture(model.StopPointCode);
        await _stoppointRepository.Update(model, Auth.User.UserID);
        var after = await _stoppointRepository.Capture(model.StopPointCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Stop Point",
            EntityId = model.StopPointCode,
            ReferenceId = model.StopPointCode,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string id)
    {
        var before = await _stoppointRepository.Capture(id);
        await _stoppointRepository.Remove(id, Auth.User.UserID);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master Stop Point",
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
        var results = await _stoppointRepository.GetAll(parameter);
        if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["Stop Point Code", "Description", "Picking Seq", "Last Update", "Last User"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.StopPointCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Description);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.PickingSeq);
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
