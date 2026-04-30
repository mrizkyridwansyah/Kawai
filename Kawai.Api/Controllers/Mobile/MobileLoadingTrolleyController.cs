using Kawai.Api.Services;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Kawai.Domain.DTOs.Log;
using Hangfire;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/loading-trolley")]
[ApiController]
public class MobileLoadingTrolleyController : HahaController
{
    private readonly IMobileLoadingTrolleyRepository _loadingTrolleyRepository;
    private readonly ITransactionProducer _transactionProducer;
    private readonly DataLogger _logger;

    public MobileLoadingTrolleyController(IMobileLoadingTrolleyRepository LoadingTrolleyRepository, ITransactionProducer transactionProducer, DataLogger logger)
    {
        _loadingTrolleyRepository = LoadingTrolleyRepository;
        _transactionProducer = transactionProducer;
        _logger = logger;
    }

    [HttpGet("data-trolley")]
    public async Task<IActionResult> GetDataTrolley(string trolleyNo)
    {
        var result = await _loadingTrolleyRepository.GetDataTrolley(trolleyNo);
        var grouped = result
        .GroupBy(x => new { x.PickingNo, x.RequestDescription, x.LineCode, x.LineName, x.WorkStationCode, x.WorkStationName, x.StopPointCode, x.StopPointName })
        .Select(g => new
        {
            g.Key.PickingNo,
            g.Key.RequestDescription,
            g.Key.LineCode,
            g.Key.LineName,
            g.Key.WorkStationCode,
            g.Key.WorkStationName,
            g.Key.StopPointCode,
            g.Key.StopPointName,
            Details = g.Select(x => new
            {
                x.RefNo,
                x.ItemCode,
                x.ItemName,
                x.BarcodeNo,
                x.LotNo,
                x.Qty,
                x.StatusScan
            }).ToList()
        })
        .FirstOrDefault();
        return Success(grouped);
    }

    [HttpPost("scan")]
    public async Task<IActionResult> ScanBarcode(MobileLoadingTrolley model)
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

        try
        {
            _transactionProducer.Publish<MobileLoadingTrolley>(message);
            return Pending(message);
        }
        catch (Exception ex)
        {
            throw new Exception("RabbitMQ unavailable: " + ex.Message);
        }
    }

    [HttpPost("complete")]
    public async Task<IActionResult> Save(MobileLoadingTrolleyComplete model)
    {
        var before = await _loadingTrolleyRepository.CapturePicking(model.PickingNo);

        await _loadingTrolleyRepository.CompleteLoading(model, Auth.User.UserID);

        var after = await _loadingTrolleyRepository.CapturePicking(model.PickingNo);

        /*
         * DISINI NIH TEMPAT BUAT CODE REQUEST API EKTERNAL AMR
        //BackgroundJob.Enqueue<IRobotService>(service => service.SendRobotRequest(model));
        */

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Loading Trolley",
            EntityId = model.PickingNo.ToString(),
            ReferenceId = model.PickingNo.ToString(),
            Before = before,
            After = after,
            Activity = "Complete Mobile Loading Trolley",
            Action = DataLogAction.Update
        });
        return Success(after);
    }
}
