using Kawai.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/order-entry")]
[ApiController]
public class OrderEntryController : HahaController
{
    private readonly IOrderEntryRepository _orderEntryRepository;

    public OrderEntryController(IOrderEntryRepository orderEntryRepository)
    {
        _orderEntryRepository = orderEntryRepository;
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(
        [FromQuery] string keyword,
        [FromQuery] string custCode,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] string ids)
    {
        var results = await _orderEntryRepository.GetFilterDDL(custCode, dateFrom, dateTo);

        if (!string.IsNullOrWhiteSpace(keyword))
        {
            var key = keyword.Trim();
            results = results
                .Where(x => !string.IsNullOrEmpty(x.PO_No) && x.PO_No.Contains(key, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.PO_No)).ToList();
        }

        return Success(results);
    }
}
