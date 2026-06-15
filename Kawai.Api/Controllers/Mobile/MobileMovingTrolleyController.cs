using Kawai.Api.Services;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/moving-trolley")]
[ApiController]
public class MobileMovingTrolleyController : HahaController
{
    private readonly IMobileMovingTrolleyRepository _movingTrolleyRepository;
    private readonly ITransactionProducer _transactionProducer;

    public MobileMovingTrolleyController(IMobileMovingTrolleyRepository MovingTrolleyRepository, ITransactionProducer transactionProducer)
    {
        _movingTrolleyRepository = MovingTrolleyRepository;
        _transactionProducer = transactionProducer;
    }

    [HttpGet("data-trolley")]
    public async Task<IActionResult> GetDataTrolley(string trolleyNo)
    {
        var results = await _movingTrolleyRepository.GetDataTrolley(trolleyNo);
        var grouped = results
        .GroupBy(x => new { x.RequestNo })
        .Select(g => new
        {
            g.Key.RequestNo,
            Details = g.Select(x => new
            {
                x.WarehouseCode,
                x.WarehouseName,
                x.AreaCode,
                x.AreaName,
                x.AddressCode,
                x.AddressName,
                x.ItemCode,
                x.ItemName,
                x.TotalQty
            }).ToList()
        })
        .FirstOrDefault();

        return Success(grouped);
    }

    [HttpGet("data-stop-point")]
    public async Task<IActionResult> GetDataStopPoint(string stopPoint)
    {
        var result = await _movingTrolleyRepository.GetDataStopPoint(stopPoint);
        return Success(result);
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit(MobileMovingTrolley model)
    {
        var message = new StockTransactionMessage<MobileMovingTrolley>
        {
            Token = Auth.Token,
            AuthUserId = Auth.User.UserID,
            BroadcastBaseOn = "TOKEN",
            TimeStamp = EpochDateTime.Now,
            TransactionType = "MOVING-TROLLEY-MOBILE",
            FormatMessage = "Moving Trolley Mobile",
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
            _transactionProducer.Publish<MobileMovingTrolley>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }
}
