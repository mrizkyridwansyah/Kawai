using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/iqc-sample")]
[ApiController]
public class MobileIQCSampleController : HahaController
{
    private readonly IMobileIQCSampleRepository _iqcSampleRepository;
    private readonly DataLogger _logger;

    public MobileIQCSampleController(IMobileIQCSampleRepository iqcSampleRepository, DataLogger logger)
    {
        _iqcSampleRepository = iqcSampleRepository;
        _logger = logger;
    }

    [HttpGet("list-sample")]
    public async Task<IActionResult> GetListSample(long id)
    {
        var result = await _iqcSampleRepository.GetListSample(id);
        return Success(result);
    }

    [HttpGet("data-sample")]
    public async Task<IActionResult> GetDataSampleBarcode(long id, string barcodeNo)
    {
        var result = await _iqcSampleRepository.GetDataSampleBarcode(id, barcodeNo);
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileIQCSample model)
    {
        var before = model.InspectionId.HasValue ? await _iqcSampleRepository.Capture(model.InspectionId.Value) : null;

        await _iqcSampleRepository.Save(model, Auth.User.UserID);

        var after = await _iqcSampleRepository.Capture(model.InspectionId.Value);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile IQC Sample",
            EntityId = model.InspectionId.ToString(),
            ReferenceId = model.InspectionId.ToString(),
            Before = before,
            After = after,
            Activity = "Save Mobile IQC Sample",
            Action = DataLogAction.Create
        });

        return Success(after, "Data saved successfully!");
    }
}
