using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/receipt")]
[ApiController]
public class MobileReceiptController : HahaController
{
    private readonly IReceiptRepository _receiptRepository;
    private readonly ITransactionProducer _transactionProducer;
    private readonly DataLogger _logger;

    public MobileReceiptController(IReceiptRepository receiptRepository, ITransactionProducer transactionProducer, DataLogger logger)
    {
        _receiptRepository = receiptRepository;
        _transactionProducer = transactionProducer;
        _logger = logger;
    }


    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearchReceipt(string keyword, string status, string ids)
    {
        var results = await _receiptRepository.DDLSearchReceipt(keyword, status);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.Id.ToString())).ToList();
        }

        return Success(results.Take(100));
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetDetail(long id)
    {
        var header = _receiptRepository.GetDataHeader(id);
        var detail = _receiptRepository.GetListDetail(id);
        var detailBarcode = _receiptRepository.GetListDetailBarcode(id);

        await Task.WhenAll(header, detail, detailBarcode);

        var resultDetail = detail.Result.Select(p => new
        {
            p.Id,
            p.ReceiptId,
            p.PONumber,
            p.ItemCode,
            p.ItemName,
            p.UnitClsCode,
            p.UnitClsName,
            p.ExpectedQty,
            p.TotalPacking,
            p.ReceiptQty,
            p.IQCResult,
            DetailBarcodes = detailBarcode.Result.Where(d => d.ReceiptDetailId == p.Id).ToList()
        });

        var result = new
        {
            header.Result.Id,
            header.Result.ReceiptNo,
            header.Result.ReceiptDate,
            header.Result.DNNumber,
            header.Result.SupplierCode,
            header.Result.SupplierName,
            header.Result.DNDate,
            header.Result.BCNumber,
            header.Result.BCType,
            header.Result.BCDate,
            header.Result.VehicleNo,
            header.Result.LastUpdate,
            header.Result.LastUser,
            Details = resultDetail
        };

        return Success(result);
    }

    [HttpGet("data-barcode")]
    public async Task<IActionResult> GetDataBarcode(long id, string barcodeNo)
    {
        var result = await _receiptRepository.GetDataBarcode(id, barcodeNo);
        return Success(result);
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(MobileReceipt model)
    {
        if (model.Qty != model.QtyVerify)
        {
            var message = new StockTransactionMessage<MobileReceipt>
            {
                AuthUserId = Auth.User.UserID,
                TimeStamp = EpochDateTime.Now,
                TransactionType = "RECEIPT-VERIFY-MOBILE",
                FormatMessage = "Receipt Barcode No. : " + model.BarcodeNo,
                Payload = model,
                LogContext = new LogContext
                {
                    Method = HttpContext.Request.Method,
                    RequestPath = HttpContext.Request.Path,
                    RemoteAddr = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString(),
                    UserAgent = HttpContext.Request.Headers.UserAgent.ToString(),
                    UserID = Auth.User.UserID,
                    FullName = Auth.User.FullName
                }
            };

            _transactionProducer.Publish<MobileReceipt>(message);
            return Pending(message);

        }
        else
        {
            var before = await _receiptRepository.CaptureDataBarcode(model.Id);
            await _receiptRepository.Verify(model, false, Auth.User.UserID);
            var after = await _receiptRepository.CaptureDataBarcode(model.Id);

            await _logger.SaveDataLog(new DataLogDto
            {
                DocumentType = "Mobile - Receipt",
                EntityId = model.Id.ToString(),
                ReferenceId = model.BarcodeNo,
                Before = before,
                After = after,
                Action = DataLogAction.Update,
                Activity = "Verify Receiving Barcode"
            });

            return Success(after, "Data berhasil diverifikasi!");
        }
    }
}
