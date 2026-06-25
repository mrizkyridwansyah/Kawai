using Kawai.Api.Services;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Authorize]
[Route("api/mobile/barcode-split")]
[ApiController]
public class MobileBarcodeSplitController : HahaController
{
    private readonly IMobileBarcodeSplitRepository _barcodeSplitRepository;
    private readonly ITransactionProducer _transactionProducer;
    public MobileBarcodeSplitController(IMobileBarcodeSplitRepository barcodeSplitRepository, ITransactionProducer transactionProducer)
    {
        _barcodeSplitRepository = barcodeSplitRepository;
        _transactionProducer = transactionProducer;
    }

    [HttpGet("data-barcode")]
    public async Task<IActionResult> GetDataBarcode(string barcodeNo)
    {
        var result = await _barcodeSplitRepository.GetDataBarcode(barcodeNo);
        return Success(result);
    }

    [HttpGet("history-split")]
    public async Task<IActionResult> GetHistorySplit(string barcodeNo)
    {
        var result = await _barcodeSplitRepository.GetHistorySplit(barcodeNo);
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileBarcodeSplit model)
    {
        var message = new StockTransactionMessage<MobileBarcodeSplit>
        {
            Token = Auth.Token,
            AuthUserId = Auth.User.UserID,
            BroadcastBaseOn = "TOKEN",
            TimeStamp = EpochDateTime.Now,
            TransactionType = "BARCODE-SPLIT-MOBILE",
            FormatMessage = "Barcode Split Mobile",
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

        try
        {
            _transactionProducer.Publish<MobileBarcodeSplit>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }
}
