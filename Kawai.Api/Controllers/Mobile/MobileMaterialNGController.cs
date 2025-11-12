using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Kawai.Domain.Models.Mobile;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile/material-ng")]
[ApiController]
public class MobileMaterialNGController : HahaController
{
    private readonly IMobileMaterialNGRepository _materialNGRepository;
    private readonly DataLogger _logger;

    public MobileMaterialNGController(IMobileMaterialNGRepository materialNGRepository, DataLogger logger)
    {
        _materialNGRepository = materialNGRepository;
        _logger = logger;
    }

    [HttpGet("data-ng")]
    public async Task<IActionResult> GetDataNGBarcode(string barcodeNo)
    {
        var result = await _materialNGRepository.GetDataNGBarcode(barcodeNo);
        return Success(result);
    }

    [HttpPost("save")]
    public async Task<IActionResult> Save(MobileMaterialNG model)
    {
        var before = model.InspectionId.HasValue ? await _materialNGRepository.Capture(model.InspectionId.Value) : null;

        await _materialNGRepository.Save(model, Auth.User.UserID);

        var after = await _materialNGRepository.Capture(model.InspectionId.Value);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Mobile Material NG",
            EntityId = model.InspectionId.ToString(),
            ReferenceId = model.InspectionId.ToString(),
            Before = before,
            After = after,
            Activity = "Save Mobile Material NG",
            Action = DataLogAction.Create
        });

        return Success(after, "Data saved successfully!");
    }
}
