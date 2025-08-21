using Kawai.Domain.Interfaces.Mobile;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Route("api/mobile")]
[ApiController]
public class MobileController : HahaController
{
    private readonly IMobileRepository _mobileRepository;
    public MobileController(IMobileRepository mobileRepository)
    {
        _mobileRepository = mobileRepository;
    }

    [HttpGet("last-version")]
    public async Task<IActionResult> GetLastVersion()
    {
        var result = await _mobileRepository.GetLastVersion();
        return Success(result);
    }

}
