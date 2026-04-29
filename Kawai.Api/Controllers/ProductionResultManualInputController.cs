using Kawai.Domain;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/production/manual-input")]
[ApiController]
public class ProductionResultManualInputController : HahaController
{
    private readonly IProductionResultManualInputRepository _productionResultManualInputRepository;
    private readonly DataLogger _logger;

    public ProductionResultManualInputController(IProductionResultManualInputRepository productionResultManualInputRepository, DataLogger logger)
    {
        _productionResultManualInputRepository = productionResultManualInputRepository;
        _logger = logger;
    }

    [HttpPost("list-header")]
    public async Task<IActionResult> ListHeader([FromBody] RequestParameter parameter)
    {
        var results = await _productionResultManualInputRepository.GetListHeader(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("list-detail")]
    public async Task<IActionResult> ListDetail([FromBody] List<ProductionResultManualInputModel> models)
    {
        var results = await _productionResultManualInputRepository.GetListDetail(models);
        return Success(results);
    }

    [HttpPost("list-stock")]
    public async Task<IActionResult> ListStock([FromBody] RequestParameter parameter)
    {
        var results = await _productionResultManualInputRepository.GetListStock(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save([FromBody] List<ProductionResultManualInputModel> models)
    {
        var logs = new List<DataLogDto>();

        if (models == null || !models.Any())
            return Invalid("Invalid Request Data");                                                        

        foreach (var item in models)
        {
            var before = await _productionResultManualInputRepository.Capture(item.ProductionId);
            logs.Add(new DataLogDto
            {
                DocumentType = "Production Result Manual Input",
                EntityId = item.ProductionId.ToString(),
                ReferenceId = item.ProductionId.ToString(),
                Before = before,
                After = null,
                Action = DataLogAction.Update,
                Activity = "Save Production Result Manual Input"
            });
        }

        await _productionResultManualInputRepository.Save(models, Auth.User.UserID);

        foreach (var log in logs)
        {
            var after = await _productionResultManualInputRepository.Capture(log.EntityId.ToInt32());
            log.After = after;

            await _logger.SaveDataLog(log);
        }

        return Success();
    }

    

   
     }
