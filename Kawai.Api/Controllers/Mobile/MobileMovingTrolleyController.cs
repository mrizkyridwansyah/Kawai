using Hangfire;
using Kawai.Api.Services;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Interfaces.Robot;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Authorize]
[Route("api/mobile/moving-trolley")]
[ApiController]
public class MobileMovingTrolleyController : HahaController
{
    private readonly IMobileMovingTrolleyRepository _movingTrolleyRepository;
    private readonly IRobotRepository _robotRepository;
    private readonly ITransactionProducer _transactionProducer;
    private readonly IRobotService _robotService;

    public MobileMovingTrolleyController(IMobileMovingTrolleyRepository MovingTrolleyRepository, IRobotRepository robotRepository, ITransactionProducer transactionProducer, IRobotService robotService)
    {
        _movingTrolleyRepository = MovingTrolleyRepository;
        _robotRepository = robotRepository;
        _transactionProducer = transactionProducer;
        _robotService = robotService;
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

    [HttpPost("testing-amr-moving")]
    public async Task<IActionResult> TestingAMRMoving(MobileMovingTrolley model)
    {
        await _movingTrolleyRepository.CheckValidation(model);

        var scanInfo = await _robotRepository.GetSupplyScanRequestInfo(model.RequestNo);

        if (scanInfo != null && scanInfo.LineAMRCls == "1" && scanInfo.WSAMRCls == "0")
        {
            await _robotService.SendRequestSubLine(model.RequestNo, model.TrolleyNo, model.StopPoint, Auth.User.UserID);
        }

        return Success();
    }

    [HttpPost("submit")]
    public async Task<IActionResult> Submit(MobileMovingTrolley model)
    {
        await _movingTrolleyRepository.CheckValidation(model);

        var scanInfo = await _robotRepository.GetSupplyScanRequestInfo(model.RequestNo);
        /*
         * Kalo Flag AMR Cls di Manufacture_Line = 1 tapi Flag AMR Cls di WorkstationLineSetting = 0 maka saat loading trolley harus kirim ke AMR.
         */
        if (scanInfo != null && scanInfo.LineAMRCls == "1" && scanInfo.WSAMRCls == "0")
        {
            await _robotService.SendRequestSubLine(model.RequestNo, model.TrolleyNo, model.StopPoint, Auth.User.UserID);
        }

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
