using ClosedXML.Excel;
using Kawai.Api.Services;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/report/expiredlist")]
[ApiController]
public class ExpiredListController : HahaController
{
    private readonly IExpiredListRepository _expiredListRepository;

    public ExpiredListController(IExpiredListRepository expiredListRepository)
    {
        _expiredListRepository = expiredListRepository;
    }

    //[HttpPost]
    [HttpPost("view")]
    public async Task<IActionResult> GetList([FromBody] RequestParameter parameter)
    {
        var results = await _expiredListRepository.GetList(parameter);
        return DataTableResult(parameter, results);
        //return Ok(results);
    }

    [HttpPost("export/excel")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestParameter parameter)
    {
        var results = await _expiredListRepository.GetList(parameter);
        if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        //List<string> headers = ["Item Code", "Item Name", "Line Code", "Line Name", "Expired Date", "Qty", "Unit", "Status"];
        List<string> headers =
        [
            "Warehouse Code",
            "Warehouse Name",
            "Item Code",
            "Part Number",
            "Item Name",
            "Unit",
            "Lot No",
            "Receipt Date",
            "Manufacture Date",
            "Expired Day",
            "Qty",
            "Expired Date",
            "Status"
        ];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var result in results)
        {
            rowIdx++;
            int colIdx = 1;
            var row = ws.Row(rowIdx);

            ExcelHelper.SetCell(row, colIdx++, result.WHCode);
            ExcelHelper.SetCell(row, colIdx++, result.WHName);
            ExcelHelper.SetCell(row, colIdx++, result.ItemCode);
            ExcelHelper.SetCell(row, colIdx++, result.PartNo);
            ExcelHelper.SetCell(row, colIdx++, result.ItemName);
            ExcelHelper.SetCell(row, colIdx++, result.Unit);
            ExcelHelper.SetCell(row, colIdx++, result.LotNo);
            ExcelHelper.SetCell(row, colIdx++, result.ReceiptDate?.ToString("dd MMM yyyy"));
            ExcelHelper.SetCell(row, colIdx++, result.ManufactureDate?.ToString("dd MMM yyyy"));
            ExcelHelper.SetCell(row, colIdx++, result.ExpireDay);
            ExcelHelper.SetCell(row, colIdx++, result.Qty);
            ExcelHelper.SetCell(row, colIdx++, result.ExpiredDate?.ToString("dd MMM yyyy"));
            ExcelHelper.SetCell(row, colIdx++, result.Status);
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