using ClosedXML.Excel;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/receipt")]
[ApiController]
public class ReceiptController : HahaController
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly DataLogger _logger;

    public ReceiptController(IReceiptRepository receiptRepository, DataLogger logger)
    {
        _receiptRepository = receiptRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] RequestParameter parameter)
    {
        var results = await _receiptRepository.GetList(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("data-header")]
    public async Task<IActionResult> GetDataHeader(long id)
    {
        var result = await _receiptRepository.GetDataHeader(id);
        return Success(result);
    }

    [HttpPost("list-detail")]
    public async Task<IActionResult> GetListDetail(long id)
    {
        var results = await _receiptRepository.GetListDetail(id);
        return Success(results);
    }

    [HttpPost("list-po-detail")]
    public async Task<IActionResult> GetListPODetail([FromBody] RequestParameter parameter)
    {
        var results = await _receiptRepository.GetListPODetail(parameter);
        return DataTableResult(parameter, results);
    }


    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] Receipt model)
    {
        await _receiptRepository.Create(model, Auth.User.UserID);

        var after = await _receiptRepository.Capture(model.Id.Value);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Receipt Material",
            EntityId = model.Id.ToString(),
            ReferenceId = model.ReceiptNo,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] Receipt model)
    {
        var before = await _receiptRepository.Capture(model.Id.Value);

        await _receiptRepository.Update(model, Auth.User.UserID);

        var after = await _receiptRepository.Capture(model.Id.Value);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Receipt Material",
            EntityId = model.Id.ToString(),
            ReferenceId = model.ReceiptNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });
        return Success(after);
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string factory, string supplier, DateTime? periodFrom, DateTime? periodUntil, string status, string sourceMenu, string ids)
    {
        var results = await _receiptRepository.DDLSearch(keyword, factory, supplier, periodFrom, periodUntil, status, sourceMenu, Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.Id.ToString())).ToList();
        }

        return Success(results.Take(100));
    }

    [HttpPost("print-label")]
    public async Task<IActionResult> PrintLabel(Receipt payload)
    {
        if (!payload.Id.HasValue) return Invalid("Data Receipt Invalid");

        var before = await _receiptRepository.Capture(payload.Id ?? 0);

        await _receiptRepository.PrintLabel(payload.Id ?? 0, Auth.User.UserID);

        var after = await _receiptRepository.Capture(payload.Id ?? 0);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Receipt Material",
            EntityId = (payload.Id ?? 0).ToString(),
            ReferenceId = payload.ReceiptNo,
            Before = before,
            After = after,
            Activity = "Print Label Receipt",
            Action = DataLogAction.Update
        });
        return Success(after);
    }

    [HttpPost("inquiry")]
    public async Task<IActionResult> Inquiry([FromBody] RequestParameter parameter)
    {
        var results = await _receiptRepository.Inquiry(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("inquiry-detail")]
    public async Task<IActionResult> InquiryDetail([FromBody] RequestParameter parameter)
    {
        var result = await _receiptRepository.InquiryDetail(parameter);
        return DataTableResult(parameter, result);
    }

    [HttpPost("export/excel-inquiry")]
    public async Task<IActionResult> ExportExcel([FromBody] RequestParameter parameter)
    {
        var results = await _receiptRepository.Inquiry(parameter);
        if (results == null || !results.Any()) return NoContent();

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Data");

        int rowIdx = 1;

        List<string> headers = ["Supplier", "Delivery Date", "Item Code", "Description", "DN Number", "PO Number", "BC Type", "BC Number", "BC Date", "Qty", "Unit", "Currency", "Price", "Amount"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.SupplierName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.DNDate.ToString("dd MMM yyyy"));
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ItemCode);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.ItemName);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.DNNumber);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.PONumber);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.BCType);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.BCNumber);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.BCDate.ToString("dd MMM yyyy"));
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Qty);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Currency);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Price);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.Amount);
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
