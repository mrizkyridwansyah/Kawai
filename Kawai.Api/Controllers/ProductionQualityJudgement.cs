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
[Route("api/productionjudgement/quality-judgement")]
[ApiController]
public class ProductionQualityJudgementController : HahaController
{
    private readonly IProductionQualityJudgementRepository _ProductionQualityJudgementRepository;
    private readonly DataLogger _logger;

    public ProductionQualityJudgementController(IProductionQualityJudgementRepository ProductionQualityJudgementRepository, DataLogger logger)
    {
        _ProductionQualityJudgementRepository = ProductionQualityJudgementRepository;
        _logger = logger;
    }

    [HttpPost("list-header")]
    public async Task<IActionResult> ListHeader([FromBody] RequestParameter parameter)
    {
        var results = await _ProductionQualityJudgementRepository.GetListHeader(parameter);
        return DataTableResult(parameter, results);
    }

    //[HttpPost("list-detail")]
    //public async Task<IActionResult> ListDetail([FromBody] List<ProductionQualityJudgementModel> models)
    //{
    //    var results = await _ProductionQualityJudgementRepository.GetListDetail(models);
    //    return Success(results);
    //}

    [HttpPost("save")]

    public async Task<IActionResult> Save([FromBody] List<ProductionQualityJudgementModel> models)
    {
        var logs = new List<DataLogDto>();

        if (models == null || !models.Any())
            return Invalid("Invalid Request Data");

        foreach (var item in models)
        {
            var before = await _ProductionQualityJudgementRepository.Capture(item.ProdResultID);
            logs.Add(new DataLogDto
            {
                DocumentType = "Production Quality Judgement Input",
                EntityId = item.ProdResultID.ToString(),
                ReferenceId = item.ProdResultID.ToString(),
                Before = before,
                After = null,
                Action = DataLogAction.Update,
                Activity = "Save Production Quality Judgement Manual Input"
            });
        }

        await _ProductionQualityJudgementRepository.Save(models, Auth.User.UserID);

        foreach (var log in logs)
        {
            var after = await _ProductionQualityJudgementRepository.Capture(log.EntityId.ToInt32());
            log.After = after;

            await _logger.SaveDataLog(log);
        }

        return Success(logs);
    }

    

   
     }
