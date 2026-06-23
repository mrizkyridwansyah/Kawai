using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/supply-request/bom")]
[ApiController]
public class PartMaterialRequestBomController : HahaController
{
    private readonly IPartMaterialRequestBomRepository _partMaterialRequestBomRepository;
    private readonly DataLogger _logger;

    public PartMaterialRequestBomController(IPartMaterialRequestBomRepository partMaterialRequestBomRepository, DataLogger logger)
    {
        _partMaterialRequestBomRepository = partMaterialRequestBomRepository;
        _logger = logger;
    }

    [HttpGet("data-header")]
    public async Task<IActionResult> GetDataHeader(long requestId, string itemCode)
    {
        var result = await _partMaterialRequestBomRepository.GetDataHeader(requestId, itemCode);
        return Success(result);
    }

    [HttpPost("list-header")]
    public async Task<IActionResult> ListHeader([FromBody] RequestParameter parameter)
    {
        var results = await _partMaterialRequestBomRepository.GetListHeader(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("list-detail")]
    public async Task<IActionResult> ListDetail([FromBody] List<PartMaterialRequestBomModel> models)
    {
        var results = await _partMaterialRequestBomRepository.GetListDetail(models);
        return Success(results);
    }

    [HttpPost("list-stock")]
    public async Task<IActionResult> ListStock([FromBody] RequestParameter parameter)
    {
        var results = await _partMaterialRequestBomRepository.GetListStock(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] PartMaterialRequestBomHeaderModel model)
    {
        var logs = new List<DataLogDto>();

        if (model.Details == null || !model.Details.Any())
            return Invalid("Detail Request Invalid");

        foreach (var item in model.Details)
        {
            var before = await _partMaterialRequestBomRepository.Capture(item.PONumber);
            logs.Add(new DataLogDto
            {
                DocumentType = "Part Material Request Bom",
                EntityId = item.PONumber.ToString(),
                ReferenceId = item.PONumber.ToString(),
                Before = before,
                After = null,
                Action = DataLogAction.Update,
                Activity = "Save Part Material Request Bom"
            });
        }

        await _partMaterialRequestBomRepository.Save(model, Auth.User.UserID);

        foreach (var log in logs)
        {
            var after = await _partMaterialRequestBomRepository.Capture(log.EntityId);
            log.After = after;

            await _logger.SaveDataLog(log);
        }

        return Success();
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] PartMaterialRequestBomHeaderModel model)
    {
        if (!model.RequestId.HasValue) return Invalid("Request Data Invalid");

        var before = await _partMaterialRequestBomRepository.CaptureRequest(model.RequestId.Value);

        await _partMaterialRequestBomRepository.Update(model, Auth.User.UserID);

        var after = await _partMaterialRequestBomRepository.CaptureRequest(model.RequestId.Value);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Material Request Bom",
            EntityId = model.RequestNo,
            ReferenceId = model.RequestNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update,
            Activity = "Update Part Material Request Bom"
        });

        return Success();
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(long requestId, string requestNo)
    {
        var before = await _partMaterialRequestBomRepository.CaptureRequest(requestId);

        await _partMaterialRequestBomRepository.Remove(requestId, Auth.User.UserID);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Material Request Bom",
            EntityId = requestNo,
            ReferenceId = requestNo,
            Before = before,
            After = null,
            Action = DataLogAction.Delete,
            Activity = "Delete Part Material Request Bom"
        });

        return Success();
    }
}
