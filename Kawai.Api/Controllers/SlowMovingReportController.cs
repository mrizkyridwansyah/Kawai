using ClosedXML.Excel;
using Kawai.Api.Services;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/slow-moving")]
[ApiController]

public class SlowMovingReportController : HahaController
{
    private readonly ISlowMovingReportRepository _SlowMovingReportRepository;
    private readonly DataLogger _logger;

    public SlowMovingReportController(ISlowMovingReportRepository SlowMovingReportRepository, DataLogger logger)
    {
        _SlowMovingReportRepository = SlowMovingReportRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _SlowMovingReportRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestParameter parameter)
    {
        var results = await _SlowMovingReportRepository.GetAll(parameter);
        if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;
        int colIdx = 1;

        var periods = results.First().PeriodQty.Keys.OrderByDescending(x => x).ToList();

        // =========================
        // HEADER ROW 1
        // =========================
        ws.Cell(rowIdx, colIdx).Value = "Long Stock"; ws.Range(rowIdx, colIdx, rowIdx + 1, colIdx).Merge(); colIdx++;
        ws.Cell(rowIdx, colIdx).Value = "Warehouse"; ws.Range(rowIdx, colIdx, rowIdx + 1, colIdx).Merge(); colIdx++;
        ws.Cell(rowIdx, colIdx).Value = "Item Code"; ws.Range(rowIdx, colIdx, rowIdx + 1, colIdx).Merge(); colIdx++;
        ws.Cell(rowIdx, colIdx).Value = "Item Name"; ws.Range(rowIdx, colIdx, rowIdx + 1, colIdx).Merge(); colIdx++;

        int periodStartCol = colIdx;
        ws.Cell(rowIdx, colIdx).Value = "Period";
        ws.Range(rowIdx, colIdx, rowIdx, colIdx + periods.Count - 1).Merge();
        colIdx += periods.Count;

        ws.Cell(rowIdx, colIdx).Value = "Unit"; ws.Range(rowIdx, colIdx, rowIdx + 1, colIdx).Merge(); colIdx++;
        ws.Cell(rowIdx, colIdx).Value = "Remarks"; ws.Range(rowIdx, colIdx, rowIdx + 1, colIdx).Merge();

        ExcelHelper.ApplyHeaderStyle(ws, rowIdx, rowIdx, 1, colIdx, bold: true, background: XLColor.RoyalBlue);

        // =========================
        // HEADER ROW 2 (Period)
        // =========================
        int headerRow2 = rowIdx + 1;
        colIdx = periodStartCol;

        foreach (var p in periods)
        {
            ws.Cell(headerRow2, colIdx).Value = ExcelHelper.FormatPeriod(p);
            colIdx++;
        }
        ExcelHelper.ApplyHeaderStyle(ws, headerRow2, headerRow2, periodStartCol, colIdx, bold: true, background: XLColor.RoyalBlue);

        // =========================
        // DATA
        // =========================
        rowIdx = 2;

        foreach (var r in results)
        {
            rowIdx++;
            colIdx = 1;

            ws.Cell(rowIdx, colIdx++).Value = r.LongStock;
            ws.Cell(rowIdx, colIdx++).Value = $"{r.WHCode} - {r.WHName}";
            ws.Cell(rowIdx, colIdx++).Value = r.Item_Code;
            ws.Cell(rowIdx, colIdx++).Value = r.Item_Name;

            foreach (var p in periods)
            {
                var cell = ws.Cell(rowIdx, colIdx++);
                cell.Value = r.PeriodQty.TryGetValue(p, out var v) ? v ?? 0 : 0;
                cell.Style.NumberFormat.Format = "#,##0.0000";
                cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            }

            ws.Cell(rowIdx, colIdx++).Value = r.Unit_Name;
            ws.Cell(rowIdx, colIdx).Value = r.Remarks;
        }


        // =========================
        // FINAL TOUCH
        // =========================
        ws.Columns().AdjustToContents();
        ws.RangeUsed().Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
        ws.RangeUsed().Style.Border.InsideBorder = XLBorderStyleValues.Thin;

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        var fileBytes = ms.ToArray();
        var base64File = Convert.ToBase64String(fileBytes);

        return Success(base64File);
    }
}