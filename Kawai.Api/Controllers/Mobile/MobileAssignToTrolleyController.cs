using Kawai.Api.Services;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/assign-to-trolley")]
[ApiController]
public class MobileAssignToTrolleyController : HahaController
{
    private readonly IMobileAssignToTrolleyRepository _assignToTrolleyByBarcodeRepository;
    private readonly ITransactionProducer _transactionProducer;

    public MobileAssignToTrolleyController(IMobileAssignToTrolleyRepository assignToTrolleyByBarcodeRepository, ITransactionProducer transactionProducer)
    {
        _assignToTrolleyByBarcodeRepository = assignToTrolleyByBarcodeRepository;
        _transactionProducer = transactionProducer;
    }

    [HttpGet("data-trolley")]
    public async Task<IActionResult> GetDataTrolley(string trolleyNo)
    {
        var result = await _assignToTrolleyByBarcodeRepository.GetDataTrolley(trolleyNo);
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileAssignToTrolley model)
    {
        var message = new StockTransactionMessage<MobileAssignToTrolley>
        {
            Token = Auth.Token,
            AuthUserId = Auth.User.UserID,
            BroadcastBaseOn = "TOKEN",
            TimeStamp = EpochDateTime.Now,
            TransactionType = "ASSIGN-TO-TROLLEY-BY-BARCODE-MOBILE",
            FormatMessage = "Assign To Trolley By Barcode Mobile",
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
            _transactionProducer.Publish<MobileAssignToTrolley>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }
}
