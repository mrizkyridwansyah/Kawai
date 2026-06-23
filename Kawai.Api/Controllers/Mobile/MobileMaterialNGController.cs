using Kawai.Api.Services;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Authorize]
[Route("api/mobile/material-ng")]
[ApiController]
public class MobileMaterialNGController : HahaController
{
    private readonly IMobileMaterialNGRepository _materialNGRepository;
    private readonly ITransactionProducer _transactionProducer;
    private readonly DataLogger _logger;

    public MobileMaterialNGController(IMobileMaterialNGRepository materialNGRepository, ITransactionProducer transactionProducer, DataLogger logger)
    {
        _materialNGRepository = materialNGRepository;
        _transactionProducer = transactionProducer;
        _logger = logger;
    }

    [HttpGet("data-ng")]
    public async Task<IActionResult> GetDataNGBarcode(string barcodeNo)
    {
        var result = await _materialNGRepository.GetDataNGBarcode(barcodeNo);
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileMaterialNG model)
    {
        var message = new StockTransactionMessage<MobileMaterialNG>
        {
            Token = Auth.Token,
            AuthUserId = Auth.User.UserID,
            BroadcastBaseOn = "TOKEN",
            TimeStamp = EpochDateTime.Now,
            TransactionType = "MATERIAL-NG-MOBILE",
            FormatMessage = "Material NG Mobile",
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
            _transactionProducer.Publish<MobileMaterialNG>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }
}
