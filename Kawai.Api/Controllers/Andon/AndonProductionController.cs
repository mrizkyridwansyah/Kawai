using Kawai.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Andon;

[Route("api/andon/production-control")]
[ApiController]
public class AndonProductionController : HahaController
{
    private readonly IAndonProductionRepository _andonProduction;

    public AndonProductionController(IAndonProductionRepository repo)
    {
        _andonProduction= repo;
    }


    [HttpGet("headerinfo")]
    public async Task<IActionResult> GetHeaderInfo(string Line, string Model , string Scheduledate)
    {
        var results = await _andonProduction.GetHeaderInfo(Line , Model , Scheduledate);

        if (results != null && results.Any() && !String.IsNullOrEmpty(results[0].ImageName))
        {
            Stream? image = FileStorage.GetFromImages(results[0].ImageName);
            byte[] imageByte = null;

            if (image != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    image.CopyTo(memoryStream);
                    imageByte = memoryStream.ToArray();
                }
                image.Dispose();
            }

            results[0].ImageBase64 = imageByte;
        }


        return Success(results);
    }

    [HttpGet("listschedule")]
    public async Task<IActionResult> GetInfoSchedule(string Line, string Model, string Scheduledate)
    {
        var results = await _andonProduction.GetInfoSchedule(Line, Model, Scheduledate);
        return Success(results);
    }

    [HttpGet("listtrolley")]
    public async Task<IActionResult> GetInfoTrolley (string Line, string Model, string Scheduledate)
    {
        var results = await _andonProduction.GetInfoTrolley(Line, Model, Scheduledate);
        return Success(results);
    }
}
