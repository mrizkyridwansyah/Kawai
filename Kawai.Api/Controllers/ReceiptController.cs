using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using Hangfire;
using Kawai.Api.Hub;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Data;
using Kawai.Domain;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/receipt")]
[ApiController]
public class ReceiptController : HahaController
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly NotificationService<NotifApprovalHub> _notification;
    private readonly DataLogger _logger;

    public ReceiptController(IReceiptRepository receiptRepository, NotificationService<NotifApprovalHub> notification, DataLogger logger)
    {
        _receiptRepository = receiptRepository;
        _logger = logger;
        _notification = notification;
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

    [HttpPost("list-claim-detail")]
    public async Task<IActionResult> GetListClaimDetail([FromBody] RequestParameter parameter)
    {
        var results = await _receiptRepository.GetListClaimDetail(parameter);
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

    [HttpPost("create-claim")]
    public async Task<IActionResult> CreateClaim([FromBody] Receipt model)
    {
        await _receiptRepository.CreateClaim(model, Auth.User.UserID);

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

    [HttpPatch("update-claim")]
    public async Task<IActionResult> UpdateClaim([FromBody] Receipt model)
    {
        string[] bcTypeNotRequiredRegisterNo = ["BC 2.3", "BC 2.6.2", "BC 4.0"];
        Dictionary<string, List<string>> Errors = [];

        if (!bcTypeNotRequiredRegisterNo.Contains(model.BCType) && String.IsNullOrEmpty(model.RegisterNo))
            AddError(Errors, "RegisterNo", "Register No is required for BC Type " + model.BCType);

        if (Errors.Any()) return Invalid(Errors);

        var before = await _receiptRepository.Capture(model.Id.Value);

        await _receiptRepository.UpdateClaim(model, Auth.User.UserID);

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


    [HttpPatch("check-is-details-update")]
    public async Task<IActionResult> CheckIsDetailsUpdate([FromBody] Receipt model)
    {
        var result = await _receiptRepository.CheckIsDetailsUpdate(model);
        return Success(result);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] Receipt model)
    {
        string[] bcTypeNotRequiredRegisterNo = ["BC 2.3", "BC 2.6.2", "BC 4.0"];
        Dictionary<string, List<string>> Errors = [];

        if (!bcTypeNotRequiredRegisterNo.Contains(model.BCType) && String.IsNullOrEmpty(model.RegisterNo))
            AddError(Errors, "RegisterNo", "Register No is required for BC Type " + model.BCType);

        if (Errors.Any()) return Invalid(Errors);

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

    [HttpGet("dn-ddlsearch")]
    public async Task<IActionResult> DNDDLSearch(string keyword, string factory, string supplier, DateTime? periodFrom, DateTime? periodUntil, string status, string ids)
    {
        var results = await _receiptRepository.DNDDLSearch(keyword, factory, supplier, periodFrom, periodUntil, status, Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.Id.ToString())).ToList();
        }

        return Success(results.Take(100));
    }

    [HttpGet("po-ddlsearch")]
    public async Task<IActionResult> PODDLSearch(string keyword, string factory, string supplier, string typeDate, DateTime? periodFrom, DateTime? periodUntil, bool showOptionAll, string ids, long? receiptId)
    {
        var results = await _receiptRepository.PODDLSearch(keyword, factory, supplier, typeDate, periodFrom, periodUntil, showOptionAll, Auth.User.UserID, receiptId);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.PONumber)).ToList();
        }

        return Success(results);
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

    [HttpPost("print-barcodes-using-job")]
    public async Task<IActionResult> PrintBarcodeUsingJob(long receiptId)
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

        //string key = "PrintBarcodeUsingJob_" + receiptId.ToString();
        string message = "Data Export PDF sedang diproses!";
        //var fileExport = FileStorage.GetFromExports(key);
        //if (fileExport != null)
        //{
        //    message = "-";
        //    fileExport.Dispose();

        //    string keyStorage = Guid.NewGuid().ToString();
        //    _notification.BroadCastOnlyTo([Auth.User.UserID], "FileExportPDF", new { KeyFile = key, KeyStorage = keyStorage, FileName = result.SupplierName + "_" + result.DNNumber });
        //}
        //else
        //{
        BackgroundJob.Enqueue<ExportService>(service => service.ExportPdfReceiptBarcode(result, Auth.User.UserID));
        //}

        return Pending(message: message);
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
                background: #fff;
                color: #000;
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
                border-top: 1px solid #000;
                border-left: 1px solid #000;
                border-bottom: 1px solid #000;
            	 border-right: 1px solid #000;
                background: #fff;
              }

              .shipping-lot-header {
                background: #fff;
                color: #000;
                text-align: center;
               
                border-bottom: 1px solid #000;
                padding: 3px 0;
                font-size: 6px;
              }

              .shipping-lot-number {
                text-align: center;
                font-size: 22px;
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
                background: #fff;
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

    public static ReceiptImport ReadReceiptImport(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new Exception("File kosong.");

        using var stream = new MemoryStream();
        file.CopyTo(stream);

        using var workbook = new XLWorkbook(stream);
        var ws = workbook.Worksheet(1);

        var result = new ReceiptImport();

        int lastRow = ws.LastRowUsed().RowNumber();

        if (lastRow > 10000)
            throw new Exception("Upload gagal: maksimal 10.000 baris.");

        #region HEADER

        result.Header = new ReceiptHeaderImport();

        result.Header.SupplierCode = ws.Cell("A3").GetString()?.Trim();
        result.Header.DNNumber = ws.Cell("B3").GetString()?.Trim();

        string receiptDateText = ws.Cell("C3").GetString()?.Trim();

        result.Header.BCType = ws.Cell("D3").GetString()?.Trim();
        result.Header.BCNumber = ws.Cell("E3").GetString()?.Trim();

        string bcDateText = ws.Cell("F3").GetString()?.Trim();

        var headerErrors = new List<string>();
        string supplier = result.Header.SupplierCode.Trim();
        string dnno = result.Header.DNNumber.Trim();
        string bctype = result.Header.BCType.Trim();
        string bcno = result.Header.BCNumber.Trim();

        if (string.IsNullOrWhiteSpace(result.Header.SupplierCode))
            headerErrors.Add("Supplier wajib diisi");  

        if (string.IsNullOrWhiteSpace(result.Header.DNNumber))
            headerErrors.Add("DN Number wajib diisi");

        if (string.IsNullOrWhiteSpace(result.Header.BCType))
            headerErrors.Add("BC Type wajib diisi");

        if (string.IsNullOrWhiteSpace(result.Header.BCNumber))
            headerErrors.Add("BC Number wajib diisi");

        if (!DateTime.TryParse(receiptDateText, out var receiptDate))
            headerErrors.Add("Receipt Date tidak valid");
        else
            result.Header.ReceiptDate = receiptDate;

        if (!DateTime.TryParse(bcDateText, out var bcDate))
            headerErrors.Add("BC Date tidak valid");
        else
            result.Header.BCDate = bcDate;

        // Length Validation
        if (!string.IsNullOrWhiteSpace(supplier) && supplier.Length > 15)
            headerErrors.Add("Supplier maksimal 15 karakter.");

        if (!string.IsNullOrWhiteSpace(dnno) && dnno.Length > 50)
            headerErrors.Add("DNNumber maksimal 50 karakter.");

        if (!string.IsNullOrWhiteSpace(bctype) && bctype.Length > 15)
            headerErrors.Add("BCType maksimal 15 karakter.");

        if (!string.IsNullOrWhiteSpace(bcno) && bcno.Length > 50)
            headerErrors.Add("BCNo maksimal 50 karakter.");



        result.Header.Errors = string.Join(", ", headerErrors);



        #endregion

        #region DETAIL

        int startRow = 6;

        for (int row = startRow; row <= lastRow; row++)
        {
            var detail = new ReceiptDetailImport
            {
                RowNumber = row
            };

            string po = ws.Cell(row, 1).GetValue<string>()?.Trim();
            string item = ws.Cell(row, 2).GetValue<string>()?.Trim();
            string qtyText = ws.Cell(row, 3).GetValue<string>()?.Trim();

            bool allEmpty =
                string.IsNullOrWhiteSpace(po) &&
                string.IsNullOrWhiteSpace(item) &&
                string.IsNullOrWhiteSpace(qtyText);

            if (allEmpty)
                continue;

            detail.PONumber = po;
            detail.ItemCode = item;



            // Required Validation
            if (string.IsNullOrWhiteSpace(po))
                detail.Errors += $"Row {row}, PO wajib diisi. ";

            if (string.IsNullOrWhiteSpace(item))
                detail.Errors += $"Row {row}, Item wajib diisi. ";

            // Length Validation
            if (!string.IsNullOrWhiteSpace(po) && po.Length > 50)
                detail.Errors += $"Row {row}, PO maksimal 50 karakter. ";

            if (!string.IsNullOrWhiteSpace(item) && item.Length > 25)
                detail.Errors += $"Row {row}, Item maksimal 25 karakter. ";

            // Qty Validation
            if (!decimal.TryParse(qtyText, out decimal qty))
            {
                detail.Errors += $"Row {row}, Qty harus berupa angka. ";
            }
            else
            {
                detail.ReceiptQty = qty;

                if (qty <= 0)
                    detail.Errors += $"Row {row}, Qty harus lebih besar dari 0. ";
            }

            // Custom Validation
            detail.IsValid();

            result.Details.Add(detail);
        }

        #endregion

        #region VALIDASI AKHIR

        if (!result.Details.Any())
            throw new Exception("Detail receipt tidak ditemukan.");

        var duplicates = result.Details
            .GroupBy(x => new
            {
                PONumber = x.PONumber?.Trim().ToUpper(),
                ItemCode = x.ItemCode?.Trim().ToUpper()
            })
            .Where(g => g.Count() > 1);

        foreach (var duplicate in duplicates)
        {
            foreach (var item in duplicate)
            {
                item.Errors += $"Row {item.RowNumber}, PO '{item.PONumber}' dan Item '{item.ItemCode}' duplicate dalam file. ";
            }
        }



        #endregion

        return result;
    }

    [HttpPost("import")]
    public async Task<IActionResult> Import(ImportModel payload)
    {
        try
        {
            Stopwatch timer = new();
            timer.Start();

            // Read Excel
            var importData = ReadReceiptImport(payload.File);

            #region VALIDASI EXCEL

            bool hasHeaderError =
                !string.IsNullOrWhiteSpace(importData.Header?.Errors);

            bool hasDetailError =
                importData.Details.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Errors));

            if (hasHeaderError || hasDetailError)
            {
                timer.Stop();

                return ImportInvalid(
                    "DATA IMPORT TIDAK VALID",
                    importData
                );
            }

            #endregion

           

            #region VALIDASI DATABASE

            var dtDetail = DataTableHelper.ToDataTable(importData.Details);

            var validateResult = await _receiptRepository.ValidateImport(
                importData.Header,
                dtDetail,
                Auth.User.UserID);

            // update header
            if (validateResult.Header != null)
            {
                importData.Header.Errors =
                    validateResult.Header.Errors;
            }

            // update detail
            importData.Details = validateResult.Details;

            bool hasDbHeaderError =
                !string.IsNullOrWhiteSpace(importData.Header?.Errors);

            bool hasDbDetailError =
                importData.Details.Any(x =>
                    !string.IsNullOrWhiteSpace(x.Errors));

            if (hasDbHeaderError || hasDbDetailError)
            {
                timer.Stop();

                return ImportInvalid(
                    "DATA IMPORT TIDAK VALID",
                    importData
                );
            }

            #endregion

   

            #region EXECUTE

            if (payload.Action == "EXECUTE")
            {
                await _receiptRepository.Import(
                    importData.Header,
                    dtDetail,
                    Auth.User.UserID,
                    payload.FactoryCode);

                await _logger.SaveDataLog(new DataLogDto
                {
                    DocumentType = "Receipt",
                    EntityId = importData.Header.DNNumber,
                    ReferenceId = importData.Header.DNNumber,
                    Action = DataLogAction.Import,
                    Activity = "Import Receipt",
                    Before = null,
                    After = Newtonsoft.Json.Linq.JObject
                        .FromObject(importData)
                        .ToObject<Dictionary<string, object>>()
                });
            }

            #endregion

            timer.Stop();

            return Success(importData);
        }
        catch (Exception ex)
        {
            return Invalid(ex.Message);
        }
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(long id )
    {
        var before = await  _receiptRepository.Capture(id);

        await _receiptRepository.Remove(id);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Delete Part Receipt Material",
            EntityId = id.ToString(),
            ReferenceId = id.ToString(),
            Before = before,
            After = null,
            Action = DataLogAction.Delete,
            Activity = "Delete Part Receipt Material"
        });

        return Success();
    }



}
