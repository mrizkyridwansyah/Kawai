using Kawai.Api.Services;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/loading-trolley")]
[ApiController]
public class MobileLoadingTrolleyController : HahaController
{
    private readonly IMobileLoadingTrolleyRepository _loadingTrolleyRepository;
    private readonly ITransactionProducer _transactionProducer;

    public MobileLoadingTrolleyController(IMobileLoadingTrolleyRepository LoadingTrolleyRepository, ITransactionProducer transactionProducer)
    {
        _loadingTrolleyRepository = LoadingTrolleyRepository;
        _transactionProducer = transactionProducer;
    }

    [HttpGet("data-trolley")]
    public async Task<IActionResult> GetDataTrolley(string trolleyNo)
    {
        var result = await _loadingTrolleyRepository.GetDataTrolley(trolleyNo);
        return Success(result);
    }

    [HttpGet("data-barcode")]
    public async Task<IActionResult> GetDataBarcode(string trolleyNo, string barcodeNo)
    {
        var result = await _loadingTrolleyRepository.GetDataBarcode(trolleyNo, barcodeNo);
        var x = result.Select(p => new
        {
            p.ItemCode,
            p.ItemName,
            p.BarcodeNo,
            p.LotNo,
            p.SublotNo,
            Qty = p.CurrentQty
        }).ToList();
        return Success(x);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileLoadingTrolley model)
    {
        var message = new StockTransactionMessage<MobileLoadingTrolley>
        {
            AuthUserId = Auth.User.UserID,
            TimeStamp = EpochDateTime.Now,
            TransactionType = "LOADING-TROLLEY-MOBILE",
            FormatMessage = "Loading Trolley Mobile",
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

        _transactionProducer.Publish<MobileLoadingTrolley>(message);
        return Pending(message);
    }
}
