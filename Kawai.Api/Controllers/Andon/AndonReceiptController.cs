using Kawai.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Andon;

[Route("api/andon/receipt")]
[ApiController]
public class AndonReceiptController : HahaController
{
    private readonly IReceiptRepository _receiptRepository;

    public AndonReceiptController(IReceiptRepository receiptRepository)
    {
        _receiptRepository = receiptRepository;
    }

    [HttpGet("list")]
    public async Task<IActionResult> List()
    {
        var results = await _receiptRepository.GetListNSummary();
        return Success(results);
    }
}
