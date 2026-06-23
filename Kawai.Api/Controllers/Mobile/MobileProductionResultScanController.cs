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
[Route("api/mobile/production-result")]
[ApiController]
public class MobileProductionResultScanController : HahaController
{
    private readonly IMobileProductionResultScanRepository _prodresultscanRepository;
    private readonly ITransactionProducer _transactionProducer;
    private readonly DataLogger _logger;
    public MobileProductionResultScanController(IMobileProductionResultScanRepository prodresultscanRepository, ITransactionProducer transactionProducer, DataLogger logger)
    {
        _prodresultscanRepository = prodresultscanRepository;
        _transactionProducer = transactionProducer;
        _logger = logger;
    }

   
    [HttpGet("getbarcode")]
    public async Task<IActionResult> GetDataBarcode(string barcodeNo)
    {
        var result = await _prodresultscanRepository.GetDataBarcode(barcodeNo);
        return Success(result);
    }


    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileProductionResultSubmit model)
    {
        var before = await _prodresultscanRepository.Capture(model.BarcodeNo);

        await _prodresultscanRepository.Save(model, Auth.User.UserID);

        var after = await _prodresultscanRepository.Capture(model.BarcodeNo);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Production Result Scan",
            EntityId = model.BarcodeNo.ToString(),
            ReferenceId = model.BarcodeNo.ToString(),
            Before = before,
            After = after,
            Activity = "Save Mobile Production Result Scan",
            Action = DataLogAction.Update
        });
        return Success(after, "Data saved successfully!");


    }

}
