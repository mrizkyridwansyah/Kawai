using Hangfire;
using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/manual-trolley-assign")]
[ApiController]
public class MobileManualTrolleyAssignController : HahaController
{
    private readonly IMobileManualTrolleyAssignRepository _manualTrolleyAssignRepository;
    private readonly DataLogger _logger;

    public MobileManualTrolleyAssignController(IMobileManualTrolleyAssignRepository manualTrolleyAssignRepository, DataLogger logger)
    {
        _manualTrolleyAssignRepository = manualTrolleyAssignRepository;
        _logger = logger;   
    }

    [HttpGet("ddl-line")]
    public async Task<IActionResult> LineDDL(string keyword, string itemClass, string ids)
    {
        var results = await _manualTrolleyAssignRepository.GetLineDDL(keyword, itemClass);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.LineCode)).ToList();
        }

        return Success(results);
    }

    [HttpGet("ddl-requestno")]
    public async Task<IActionResult> DDLSearch(string keyword, string itemClass, string lineCode, string requestno)
    {
        var results = await _manualTrolleyAssignRepository.GetRequestNoDDL(keyword, itemClass, lineCode);
        if (!string.IsNullOrEmpty(requestno))
        {
            var idList = requestno.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.RequestNo)).ToList();
        }

        return Success(results);
    }

    [HttpGet("data-trolley")]
    public async Task<IActionResult> GetDataTrolley(string trolleyNo)
    {
        var result = await _manualTrolleyAssignRepository.GetDataTrolley(trolleyNo);
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileManualTrolleyAssign model)
    {
        var before = await _manualTrolleyAssignRepository.Capture(model.RequestNo);

        await _manualTrolleyAssignRepository.Save(model, Auth.User.UserID);

        var after = await _manualTrolleyAssignRepository.Capture(model.RequestNo);

        BackgroundJob.Enqueue<IRobotService>(service => service.CancelRequest(model.RequestNo, model.TrolleyNo));

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Manual Trolley Assign",
            EntityId = model.RequestNo,
            ReferenceId = model.RequestNo,
            Before = before,
            After = after,
            Activity = "Mobile Manual Trolley Assign",
            Action = DataLogAction.Update
        });

        return Success(after);
    }
}
