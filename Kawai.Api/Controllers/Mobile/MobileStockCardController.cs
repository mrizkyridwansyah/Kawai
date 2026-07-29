using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces.Mobile;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers.Mobile;

[Authorize]
[Route("api/mobile/stock-card")]
[ApiController]
public class MobileStockCardController : HahaController
{
    private readonly IMobileStockCardRepository _mobileStockCardRepository;
    private readonly DataLogger _logger;

    public MobileStockCardController(
        IMobileStockCardRepository mobileStockCardRepository,
        DataLogger logger)
    {
        _mobileStockCardRepository = mobileStockCardRepository;
        _logger = logger;
    }

    [HttpGet("list-stock")]
    public async Task<IActionResult> GetListStockCard(
        [FromQuery] string? warehouseCode,
        [FromQuery] string? itemCode)
    {
        var results = await _mobileStockCardRepository.GetListStockCard(
            itemCode);

        return Success(results);
    }

    [HttpGet("ddlsearch-itemcode")]
    public async Task<IActionResult> DDLSearch(string keyword, string ids)
    {
        var results = await _mobileStockCardRepository.GetDDLItemCode(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.ItemCode)).ToList();
        }

        return Success(results.Take(25));
    }
}