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
    public async Task<IActionResult> Save([FromBody] List<PartMaterialRequestBomModel> models)
    {
        var logs = new List<DataLogDto>();

        foreach (var item in models)
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

        await _partMaterialRequestBomRepository.Save(models, Auth.User.UserID);

        foreach (var log in logs)
        {
            var after = await _partMaterialRequestBomRepository.Capture(log.EntityId);
            log.After = after;

            await _logger.SaveDataLog(log);
        }

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
