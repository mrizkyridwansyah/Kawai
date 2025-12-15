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
[Route("api/workstation")]
[ApiController]
public class WorkStationController : HahaController
{
    private readonly IWorkStationRepository _workstationRepository;
    private readonly DataLogger _logger;

    public WorkStationController(IWorkStationRepository workstationRepository, DataLogger logger)
    {
        _workstationRepository = workstationRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _workstationRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string ids)
    {
        var results = await _workstationRepository.GetDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.WorkStationCode)).ToList();
        }

        return Success(results);
    }

    

    [HttpGet("detail")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _workstationRepository.GetData(id);
        return Success(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] WorkStation model)
    {
        await _workstationRepository.Create(model, Auth.User.UserID);

        var after = await _workstationRepository.Capture(model.WorkStationCode);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "WorkStation Master",
            EntityId = model.WorkStationCode,
            ReferenceId = model.WorkStationName,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] WorkStation model)
    {
        var before = await _workstationRepository.Capture(model.WorkStationCode);
        await _workstationRepository.Update(model, Auth.User.UserID);
        var after = await _workstationRepository.Capture(model.WorkStationCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "WorkStation Master",
            EntityId = model.WorkStationCode,
            ReferenceId = model.WorkStationCode,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string id)
    {
        var before = await _workstationRepository.Capture(id);
        await _workstationRepository.Remove(id, Auth.User.UserID);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "WorkStation Master",
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
        var results = await _workstationRepository.GetAll(parameter);
        if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["WS Code", "Description", "Register User", "Register Date",   "Last User", "Last Update"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.WorkStationCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.WorkStationName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.RegisterUser);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.RegisterDate.HasValue ? result.RegisterDate.Value.ToString("dd MMM yyyy HH:mm") : "");
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LastUser);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LastUpdate.HasValue ? result.LastUpdate.Value.ToString("dd MMM yyyy HH:mm") : "");
           
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
