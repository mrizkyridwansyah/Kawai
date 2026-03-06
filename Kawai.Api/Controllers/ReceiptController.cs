using ClosedXML.Excel;
using Kawai.Api.Services;
using Kawai.Domain;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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
    public async Task<IActionResult> PrintLabel(long receiptId)
    {
        var result = await _receiptRepository.GetDataHeader(receiptId);

        if (result == null) return Invalid("Data Receipt Invalid");

        var before = await _receiptRepository.Capture(result.Id ?? 0);

        await _receiptRepository.PrintLabel(result.Id ?? 0, Auth.User.UserID, true);

        var after = await _receiptRepository.Capture(result.Id ?? 0);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Receipt Material",
            EntityId = (result.Id ?? 0).ToString(),
            ReferenceId = result.ReceiptNo,
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

        List<string> headers = ["Receipt No", "Supplier", "Delivery Date", "Item Code", "Description", "DN Number", "PO Number", "BC Type", "BC Number", "BC Date", "Qty", "Qty Scan", "Unit", "Currency", "Price", "Amount"];
        ExcelHelper.SetHeader(ws, rowIdx, headers);

        foreach (var result in results)
        {
            rowIdx++;
            var row = ws.Row(rowIdx);
            int colIdx = 1;

            ExcelHelper.SetCell(row, colIdx, result.ReceiptNo);
            colIdx++;
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
            ExcelHelper.SetCell(row, colIdx, result.QtyScan);
            colIdx++;
            ExcelHelper.SetCell(row, colIdx, result.UnitClsDescription);
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

    [HttpPost("print-barcodes")]
    public async Task<IActionResult> LabelBarcode(long receiptId, [FromServices] RazorViewRenderer renderer)
    {
        var result = await _receiptRepository.GetDataHeader(receiptId);

        if (result == null) return Invalid("Data Receipt Invalid");

        var before = await _receiptRepository.Capture(result.Id ?? 0);

        await _receiptRepository.PrintLabel(result.Id ?? 0, Auth.User.UserID, false);

        var after = await _receiptRepository.Capture(result.Id ?? 0);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Receipt Material",
            EntityId = (result.Id ?? 0).ToString(),
            ReferenceId = result.ReceiptNo,
            Before = before,
            After = after,
            Activity = "Print Label Receipt",
            Action = DataLogAction.Update
        });

        var results = await _receiptRepository.GetListBarcodeDetail(receiptId);
        var renderedLabels = new List<string>();

        foreach (var item in results)
        {
            var model = new LabelBarcodeDetailDto
            {

                BarcodeNo = item.BarcodeNo,
                ReceiptNo = item.ReceiptNo,
                FromCompany = item.FromCompany,
                ToCompany = item.ToCompany,
                PONumber = item.PONumber,
                ShippingLot = item.ShippingLot,
                ItemCode = item.ItemCode,
                ItemName = item.ItemName,
                Qty = item.Qty,
                DeliveryDate = item.DeliveryDate,
                DNNumber = item.DNNumber,
                ShippingLabelNo = item.ShippingLabelNo,

            };

            var html = await renderer.RenderAsync(
                "Templates/PrintBarcode.cshtml",
                model);

            renderedLabels.Add(html);
        }

        var fullHtml = BuildA4Html(renderedLabels);
        var pdfBytes = await renderer.GeneratePdfAsync(fullHtml);
        Response.Headers.Add("Access-Control-Expose-Headers", "Content-Disposition");
        return File(pdfBytes, "application/pdf", result.SupplierName + "_" + result.DNNumber);
    }

    protected string BuildA4Html(List<string> labelHtmls)
    {
        var sb = new StringBuilder();

        sb.Append("""
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset="utf-8" />
                        <style>
                        @page {
                          size: A4;
                          margin: 10mm;
                        }

                        body {
                          margin: 0;
                          font-family: Arial, sans-serif;
                        }

                        .page {
                          width: 190mm;
                          height: 277mm;
                          display: grid;
                          grid-template-columns: repeat(2, 1fr);
                          grid-template-rows: repeat(4, 1fr);
                          gap: 5mm;
                                    /* pastikan TIDAK ada kotak */
                          border: none;
                          outline: none;
                          box-shadow: none;
                          page-break-after: always;
                        }

                        
                

              .label {
                width: 321px;
                margin: 5px auto;
                background: #fff;
                position: relative;
              }

              table {
                width: 100%;
                border-collapse: collapse;
              }

              td {
                padding: 0;
                vertical-align: top;
              }

              /* ===== HEADER ===== */
              .header {
                background: #0a4f5e;
                color: #fff;
                font-weight: bold;
                height: 25px;
              }

              .header-title {
                font-size: 10px;
                padding-left: 5px;
            	padding-top: 8px;

              }

              .header-page {
                width: 90px;
                text-align: center;
                font-size: 10px;
            	padding-top: 8px;
              }

              /* ===== SHIPPING LOT (OVERLAY) ===== */
              .shipping-lot {
                position: absolute;
                top: 0;
                right: 0;
                width: 50px;
                border-left: 1px solid #000;
                border-bottom: 1px solid #000;
            	 border-right: 1px solid #000;
                background: #fff;
              }

              .shipping-lot-header {
                background: #0a4f5e;
                color: #fff;
                text-align: center;
                padding: 3px 0;
                font-size: 6px;
              }

              .shipping-lot-number {
                text-align: center;
                font-size: 15px;
                font-weight: bold;
                padding: 4px 0;
                margin: 2px;
              }

              /* ===== FROM / TO ===== */
              .fromto td {
                width: 50%;
                padding: 2px 3px;
                border-top: 1px solid #000;
                border-bottom: 1px solid #000;
              }

              .fromto td:first-child {
                border-right: 1px solid #000;
              }

              .small {
              margin-top: 2px;
                font-size: 6px;
                font-weight: bold;
              }

              .big {
                margin-top: 3px;
                font-size: 9px;
                font-weight: bold;
              }

              /* ===== CONTENT ===== */
              .content td {
                padding: 5px;

              }

              .detail td {
                font-size: 10px;
                padding: 3px;


              }

              .lbl {
                width: 70px;

                font-weight: bold;
              }

              .colon {
                width: 5px;
              }

              .qr {
                text-align: right;
              }

              .qr img {
                width: 95px;
                height: 95px;

              }

              /* ===== FOOTER ===== */
              .footer {
                background: #efefef;
                border-top: 1px solid #000;
              }

              .footer td {
                width: 50%;
                padding: 3px 4px;
              }

              .footer td:first-child {
                border-right: 1px solid #000;
              }

              .footer-title {
              margin-top: 2px;
                font-size: 7px;
                font-weight: bold;
              }

              .footer-value {
                margin-top: 8px;
            	 font-size: 10px;
                font-weight: bold;
              }
                        </style>
                    </head>
                    <body>
            """);

        foreach (var chunk in labelHtmls.Chunk(8))
        {
            sb.Append("<div class='page'>");

            foreach (var label in chunk)
            {
                sb.Append("<div class='label'>");
                sb.Append(label);
                sb.Append("</div>");
            }

            sb.Append("</div>");
        }

        sb.Append("""
                    </body>
                    </html>
        """);

        return sb.ToString();
    }

}
