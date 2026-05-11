using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Robot;
using Kawai.Domain.Models.Robot;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Kawai.Api.Robot.Controllers;

[Authorize(AuthenticationSchemes = "Basic")]
[ApiController]
[Route("api/robot")]
public class RobotController : HahaController
{
    private readonly IRobotRepository _robotRepository;
    private readonly DataLogger _logger;

    public RobotController(IRobotRepository robotRepository, DataLogger logger)
    {
        _robotRepository = robotRepository;
        _logger = logger;
    }

    [HttpPost("set-trolley")]
    public async Task<IActionResult> SetTrolley([FromBody] SetTrolleyRequest payload)
    {
        await _robotRepository.SetTrolleyAsync(payload);
        var after = await _robotRepository.CaptureSetTrolley(payload);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Robot - Set Trolley",
            EntityId = payload.RequestID,
            ReferenceId = payload.TrolleyNo,
            Before = null,
            After = after,
            Action = DataLogAction.Update
        });

        return Success(message: "Set Trolley success");
    }

    [HttpPost("move-trolley")]
    public async Task<IActionResult> MoveTrolley([FromBody] MovingTrolleyRequest payload)
    {
        var before = await _robotRepository.CaptureStockTrolley(payload.TrolleyNo);

        await _robotRepository.MoveTrolley(payload);

        var after = await _robotRepository.CaptureStockTrolley(payload.TrolleyNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Robot - Move Trolley",
            EntityId = payload.TrolleyNo,
            ReferenceId = payload.TrolleyNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });

        return Success(message: "Move Trolley Success");
    }

    [HttpPost("empty-trolley")]
    public async Task<IActionResult> EmptyTrolley([FromBody] EmptyTrolleyRequest payload)
    {
        var data = new EmptyTrolley
        {
            TrolleyNo = payload.TrolleyNo,
            NewRefNo = "",
            PickingNo = "",
        };

        var before = await _robotRepository.CaptureStockTrolley(payload.TrolleyNo);
        await _robotRepository.EmptyTrolleyAsync(data);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Robot - Empty Trolley",
            EntityId = payload.TrolleyNo,
            ReferenceId = payload.TrolleyNo,
            Before = before,
            After = null,
            Action = DataLogAction.Delete,
            Activity = "Empty Trolley By Robot"
        });

        if (string.IsNullOrEmpty(data.NewRefNo))
        {
            var after = await _robotRepository.CaptureStockTrolley(data.NewRefNo);

            await _logger.SaveDataLog(new DataLogDto
            {
                DocumentType = "Robot - Empty Trolley",
                EntityId = payload.TrolleyNo,
                ReferenceId = payload.TrolleyNo,
                Before = null,
                After = after,
                Action = DataLogAction.Update,
                Activity = "Empty Trolley By Robot"
            });
        }

        return Success(message: "Empty Trolley (" + payload.TrolleyNo + ") Success");
    }

    [HttpGet("list-data")]
    public async Task<IActionResult> GetListDetail(string reqId)
    {
        var results = await _robotRepository.GetListData(reqId);
        var finalResults = results
        .GroupBy(x => new { x.RequestSendID, x.LineCode, x.WorkStationCode, x.ProductionDate, x.Model, x.TrolleyCls, x.TrolleyNo, x.PickingTime })
        .Select(g => new
        {
            g.Key.RequestSendID,
            g.Key.LineCode,
            g.Key.WorkStationCode,
            ProductionDate = g.Key.ProductionDate.ToString("yyyy-MM-dd"),
            g.Key.Model,
            g.Key.TrolleyCls,
            g.Key.TrolleyNo,
            PickupDatetime = g.Key.PickingTime,
            Details = g.OrderBy(x => x.PickupSequence).Select(x => new
            {
                x.StopPoint,
                x.PickupSequence,
                x.Status
            }).Distinct().ToList()
        }).ToList(); 
        
        return Success(finalResults);
    }


}
