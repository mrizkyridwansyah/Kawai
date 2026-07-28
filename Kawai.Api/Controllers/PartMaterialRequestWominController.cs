using Hangfire;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Security.Cryptography;
using System.Text;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/supply-request/womin")]
[ApiController]
public class PartMaterialRequestWominController : HahaController
{
    private readonly IPartMaterialRequestWominRepository _partMaterialRequestWominRepository;
    private readonly DataLogger _logger;

    public PartMaterialRequestWominController(IPartMaterialRequestWominRepository partMaterialRequestWominRepository, DataLogger logger)
    {
        _partMaterialRequestWominRepository = partMaterialRequestWominRepository;
        _logger = logger;
    }

    [HttpPost("list-header")]
    public async Task<IActionResult> ListHeader([FromBody] RequestParameter parameter)
    {
        var results = await _partMaterialRequestWominRepository.GetListHeader(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("getdetail")]
    public async Task<IActionResult> GetDataRequirement(long id)
    {
        var result = await _partMaterialRequestWominRepository.GetDataRequirement(id);
        return Success(result);
    }

    [HttpPatch("updatereqqty")]
    public async Task<IActionResult> Update([FromBody] PartMaterialRequestWominEditModel model)
    {
        var before = await _partMaterialRequestWominRepository.CaptureRequirement(model.IDSeq);
        await _partMaterialRequestWominRepository.UpdateReq(model, Auth.User.UserID);
        var after = await _partMaterialRequestWominRepository.CaptureRequirement(model.IDSeq);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Material Request Womin - Edit Requirement",
            EntityId = model.IDSeq.ToString(),
            ReferenceId = model.IDSeq.ToString(),
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }


    [HttpPost("check-valid")]
    public async Task<IActionResult> CheckValidation([FromBody] List<PartMaterialRequestWominModel> models)
    {
        if (models == null || !models.Any())
            return Invalid("Invalid Request Data. Please Choose Schedule!");

        await _partMaterialRequestWominRepository.CheckValidGenerateDetail(models);
        return Success();
    }

    [HttpPost("list-detail")]
    public async Task<IActionResult> ListDetail([FromBody] List<PartMaterialRequestWominModel> models)
    {
        if (models == null || !models.Any())
            return Invalid("Invalid Request Data. Please Choose Schedule!");

        var results = await _partMaterialRequestWominRepository.GetListDetail(models);
        return Success(results);
    }

    [HttpPost("list-stock")]
    public async Task<IActionResult> ListStock([FromBody] RequestParameter parameter)
    {
        var results = await _partMaterialRequestWominRepository.GetListStock(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("list-scan")]
    public async Task<IActionResult> ListScan([FromBody] RequestParameter parameter)
    {
        var results = await _partMaterialRequestWominRepository.GetListScan(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] List<PartMaterialRequestWominModel> models)
    {
        if (models == null || !models.Any())
            return Invalid("Invalid Request Data");

        var newRequest = await _partMaterialRequestWominRepository.Save(models, Auth.User.UserID);

        long newRequestId = 0;
        string newRequestNo = string.Empty;

        if (newRequest.TryGetValue("RequestId", out var idVal) && idVal != null)
            newRequestId = Convert.ToInt64(idVal);

        if (newRequest.TryGetValue("RequestNo", out var noVal) && noVal != null)
            newRequestNo = noVal.ToString();

        if(newRequestId != 0 && !string.IsNullOrEmpty(newRequestNo))
        {
            var after = await _partMaterialRequestWominRepository.CaptureRequest(newRequestId);

            await _logger.SaveDataLog(new DataLogDto
            {
                DocumentType = "Part Material Request Womin",
                EntityId = newRequestId.ToString(),
                ReferenceId = newRequestNo,
                Before = null,
                After = after,
                Action = DataLogAction.Create,
                Activity = "Save Part Material Request Womin"
            });
        }

        return Success();
    }



    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(long requestId, string requestNo)
    {
        var before = await _partMaterialRequestWominRepository.CaptureRequest(requestId);

        await _partMaterialRequestWominRepository.Remove(requestId, Auth.User.UserID);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Material Request Womin",
            EntityId = requestNo,
            ReferenceId = requestNo,
            Before = before,
            After = null,
            Action = DataLogAction.Delete,
            Activity = "Delete Part Material Request Womin"
        });

        return Success();
    }

    [HttpPost("print-womin-using-job")]
    public async Task<IActionResult> PrintBarcodeUsingJob(long requestId)
    {
        string key = Guid.NewGuid().ToString();
        BackgroundJob.Enqueue<ExportService>(service => service.ExportWomin(requestId,  Auth.Token, key));
        return Pending(message: "Data Export sedang diproses!");
    }


}
