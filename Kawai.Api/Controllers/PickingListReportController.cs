using ClosedXML.Excel;
using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/report/picking-list-report")]
[ApiController]
public class PickingListReportController : HahaController
{
    private readonly IPickingListReportRepository _PickingListReportRepository;
    private readonly DataLogger _logger;

    public PickingListReportController(IPickingListReportRepository PickingListReportRepository, DataLogger logger)
    {
        _PickingListReportRepository = PickingListReportRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _PickingListReportRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestParameter parameter)
    {
        var results = await _PickingListReportRepository.GetAll(parameter);
        if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["Cust. Code", "Cust. Name", "Shipping Instruction No.", "Shipping Instruction Date",
                "Part Number", "Description", "Serial No.", "Address", "Picking Date", "Time", "Picking By"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.Cust_Code);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Trade_Name);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.SI_NO);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.SI_Date.HasValue ? result.SI_Date.Value.ToString("dd-MMM-yy") : "");
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Item_Code);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Item_Name);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Serial_No);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Address);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Picking_Date.HasValue ? result.Picking_Date.Value.ToString("dd-MMM-yy") : "");
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Picking_Time.HasValue ? result.Picking_Time.Value.ToString(@"hh\:mm\:ss") : "");
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Picking_Name);
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
