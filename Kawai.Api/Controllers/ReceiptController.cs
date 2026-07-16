using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Office2010.Excel;
using Hangfire;
using Kawai.Api.Hub;
using Kawai.Api.Services;
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

    [HttpGet("list-breakdown-detail")]
    public async Task<IActionResult> GetListBreakdownDetail(long receiptId, long detailId)
    {
        var results = await _receiptRepository.GetListBreakdownReceipt(receiptId, detailId);
        return Success(results);
    }

    [HttpPost("save-breakdown")]
    public async Task<IActionResult> SaveBreakdown([FromBody] ReceiptBreakdown payload)
    {
        if(payload.BreakdownDetails == null || !payload.BreakdownDetails.Any())
            return Invalid("Detail Breakdown is required");

        var before = await _receiptRepository.CaptureBreakdown(payload.ReceiptDetailId.Value);

        await _receiptRepository.SaveBreakdownReceipt(payload, Auth.User.UserID);

        var after = await _receiptRepository.CaptureBreakdown(payload.ReceiptDetailId.Value);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Receipt Breakdown",
            EntityId = payload.ReceiptDetailId.Value.ToString(),
            ReferenceId = payload.ReceiptDetailId.Value.ToString(),
            Before = before,
            After = after,
            Activity = "Save Breakdown Receipt",
            Action = DataLogAction.Update
        });

        return Success(after);
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
        string key = "ReceiptInquiryExport_" + Guid.NewGuid().ToString();
        BackgroundJob.Enqueue<ExportService>(service => service.ExportExcelReceiptInquiry(parameter, Auth.Token, key));
        return Pending(message: "Data Export Excel sedang diproses!");
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
        var models = results.Select(item => new LabelBarcodeDetailDto
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
        }).ToList();

        var fullHtml = await renderer.RenderAsync("Templates/PrintBarcodesA4.cshtml", models);
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
        BackgroundJob.Enqueue<ExportService>(service => service.ExportPdfReceiptBarcode(result, Auth.Token));
        //}

        return Pending(message: message);
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

        //if (string.IsNullOrWhiteSpace(result.Header.BCNumber))
        //    headerErrors.Add("BC Number wajib diisi");

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

        //if (!string.IsNullOrWhiteSpace(bcno) && bcno.Length > 50)
        //    headerErrors.Add("BCNo maksimal 50 karakter.");



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
    public async Task<IActionResult> Remove(long id)
    {
        var before = await _receiptRepository.Capture(id);

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

