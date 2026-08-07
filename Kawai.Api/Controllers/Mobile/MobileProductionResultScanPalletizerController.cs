using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Data.Repositories.Mobile;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Authorize]
[Route("api/mobile/production-result-palletizer")]
[ApiController]
public class MobileProductionResultScanPalletizerController : HahaController
{
    private readonly IMobileProductionResultScanPalletizerRepository _prodresultscanpalletizerRepository;
    private readonly ITransactionProducer _transactionProducer;
    private readonly DataLogger _logger;
    public MobileProductionResultScanPalletizerController(IMobileProductionResultScanPalletizerRepository prodresultscanpalletizerRepository, ITransactionProducer transactionProducer, DataLogger logger)
    {
        _prodresultscanpalletizerRepository = prodresultscanpalletizerRepository;
        _transactionProducer = transactionProducer;
        _logger = logger;
    }

   
    [HttpGet("getbarcode")]
    public async Task<IActionResult> GetDataBarcode(string barcodeNo)
    {
        var result = await _prodresultscanpalletizerRepository.GetDataBarcode(barcodeNo);
        return Success(result);
    }


    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileProductionResultSubmit model)
    {
        var before = await _prodresultscanpalletizerRepository.Capture(model.BarcodeNo);

        await _prodresultscanpalletizerRepository.Save(model, Auth.User.UserID);

        var after = await _prodresultscanpalletizerRepository.Capture(model.BarcodeNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Production Result Scan Palletizer",
            EntityId = model.BarcodeNo.ToString(),
            ReferenceId = model.BarcodeNo.ToString(),
            Before = before,
            After = after,
            Activity = "Save Mobile Production Result Scan Palletizer",
            Action = DataLogAction.Update
        });
        return Success(after, "Data saved successfully!");


    }

}
