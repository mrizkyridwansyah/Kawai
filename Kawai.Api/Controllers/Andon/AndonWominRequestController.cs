using Kawai.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Andon;

[Route("api/andon/womin-request")]
[ApiController]
public class AndonWominRequestController : HahaController
{
    private readonly IReceiptRepository _receiptRepository;

    public AndonWominRequestController(IReceiptRepository receiptRepository)
    {
        _receiptRepository = receiptRepository;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List()
    {
        var results = await _receiptRepository.GetListNSummary();
        return Success(results);
    }
}
