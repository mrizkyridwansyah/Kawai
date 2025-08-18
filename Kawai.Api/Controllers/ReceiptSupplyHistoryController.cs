using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Route("api/receipt-supply-history")]
[ApiController]
public class ReceiptSupplyHistoryController : HahaController
{
    private readonly IReceiptSupplyHistoryRepository _receiptSupplyHistoryRepository;
    public ReceiptSupplyHistoryController(IReceiptSupplyHistoryRepository receiptSupplyHistoryRepository)
    {
        _receiptSupplyHistoryRepository = receiptSupplyHistoryRepository;
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] RequestParameter parameter)
    {
        var results = await _receiptSupplyHistoryRepository.GetList(parameter);
        return DataTableResult(parameter, results);
    }
}
