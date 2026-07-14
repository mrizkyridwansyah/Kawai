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
[Route("api/inventory-report")]
[ApiController]
public class InventoryReportController : HahaController
{
    private readonly IInventoryReportRepository _InventoryReportRepository;
    private readonly DataLogger _logger;

    public InventoryReportController(IInventoryReportRepository InventoryReportRepository, DataLogger logger)
    {
        _InventoryReportRepository = InventoryReportRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _InventoryReportRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestParameter parameter)
    {
        var results = await _InventoryReportRepository.GetAll(parameter);
        if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["Warehouse", "Product Code", "Product Name",
                "Pre Month", "Receipt", "Supply", "Loss / Reject","Current (A)","Allocation (B)","Ready Stock (C = A-B)","Inventory","Remarks","User"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.Warehouse);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ProductCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ProductName);
            colIdx++;
            //ExcelHelper.SetCell(row, colIdx, result.LotNo); //14-07-2026 request summary data tanpa area & lotno
            //colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.PreMonth);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Receipt);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Supply);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LossReject);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Current);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Allocation);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Ready);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Inventory);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Remarks);
            colIdx++;
            //ExcelHelper.SetCell(row, colIdx, result.LastUpdate.HasValue ? result.LastUpdate.Value.ToString("dd MMM yyyy HH:mm") : "");
            //colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.LastUser);
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
