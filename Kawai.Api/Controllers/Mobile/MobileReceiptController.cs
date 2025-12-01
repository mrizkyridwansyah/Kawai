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
    public async Task<IActionResult> DDLSearchReceipt(string keyword, string ids)
    {
        var results = await _receiptRepository.DDLSearchReceipt(keyword, Auth.User.UserID);
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

        foreach (var barcode in detailBarcode.Result)
        {
            barcode.DNNumber = header.Result.DNNumber;
            barcode.SupplierCode = header.Result.SupplierCode;
            barcode.SupplierName = header.Result.SupplierName;
        }

        var resultDetail = detail.Result.Select(p => new
        {
            p.Id,
            p.ReceiptId,
            PONumber = p.PONumber ?? "",
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
    public async Task<IActionResult> GetDataBarcode(string barcodeNo)
    {
        var result = await _receiptRepository.GetDataBarcode(barcodeNo, Auth.User.UserID);
        return Success(result);
    }

    [HttpPost("verify")]
    public async Task<IActionResult> Verify(MobileReceipt model)
    {
        if (model.Details == null || model.Details.Count == 0)
            return Invalid("Detail Barcode tidak boleh kosong");

        var message = new StockTransactionMessage<MobileReceipt>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "RECEIPT-VERIFY-MOBILE",
            FormatMessage = "Receipt Check Mobile",
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
}
