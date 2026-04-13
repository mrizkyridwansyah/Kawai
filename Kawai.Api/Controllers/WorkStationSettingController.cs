using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
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
[Route("api/workstationsetting")]
[ApiController]
public class WorkStationSettingController : HahaController
{
    private readonly IWorkStationSettingRepository _workstationsettingRepository;
  
    private readonly DataLogger _logger;

    public WorkStationSettingController(IWorkStationSettingRepository workstationsettingRepository, DataLogger logger)
    {
        _workstationsettingRepository = workstationsettingRepository;
        _logger = logger;
        
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _workstationsettingRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("ddl-linecompany-search")]
    public async Task<IActionResult> DDLLineSearch(string keyword, string companyCode, string manufacture, string ids)
    {
        var results = await _workstationsettingRepository.GetLineCompanyDDL(keyword, companyCode, manufacture);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.LineCode)).ToList();
        }

        return Success(results);
    }
        
    
    [HttpPost("save")]
    public async Task<IActionResult> Save(WorkStationSetting model)
    {
        var before = await _workstationsettingRepository.Capture(model.LineCode);
        await _workstationsettingRepository.SaveWorkStationSetting(model,Auth.User.UserID);
        var after = await _workstationsettingRepository.Capture(model.LineCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "WorkStationSetting",
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
        var results = await _workstationsettingRepository.GetAll(parameter);
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
    [HttpPost("export/qrcode")]
    public async Task<IActionResult> ExportQRCode([FromBody] List<Dictionary<string, object>> rows)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 2;
        int qrSize = 150;

        // Style for value cells
        var style = workbook.Style;
        style.Font.Bold = true;
        style.Font.FontSize = 16;
        style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        // Style for value cells
        var style2 = workbook.Style;
        style2.Font.Bold = true;
        style2.Font.FontSize = 14;
        style2.Alignment.WrapText = true;
        style2.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        style2.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        foreach (var rowMap in rows)
        {
            int startRow = rowIdx;
            var row = ws.Row(rowIdx);

            int colIdx = 2;

            // Insert QR code in cell
            string key = rowMap.TryGetValue("Key", out var keyVal) ? keyVal?.ToString() ?? "" : "";
            ExcelHelper.InsertQRCode(ws, rowIdx, colIdx++, key, qrSize);

            // Insert value cell with style
            string value = rowMap.TryGetValue("Value", out var val) ? val?.ToString() ?? "" : "";
            string value1 = rowMap.TryGetValue("Value1", out var val1) ? val1?.ToString() ?? "" : "";
            string value2 = rowMap.TryGetValue("Value2", out var val2) ? val2?.ToString() ?? "" : "";
            string value3 = rowMap.TryGetValue("Value3", out var val3) ? val3?.ToString() ?? "" : "";
            var valueCell = row.Cell(colIdx++);
            valueCell.Value = value + Environment.NewLine + value1 + Environment.NewLine + value2 + Environment.NewLine + value3;
            valueCell.Style = style2;
            ws.Column(3).Width = 40;

            // Set outer border for range with QR + value
            var range = ws.Range(startRow, 2, rowIdx, colIdx - 1);
            range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            rowIdx += 2;
        }

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        var fileBytes = ms.ToArray();
        var base64File = Convert.ToBase64String(fileBytes);

        return Success(base64File);
    }

}
