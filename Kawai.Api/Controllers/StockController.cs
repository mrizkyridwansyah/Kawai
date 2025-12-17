using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Route("api/stock")]
[ApiController]
public class StockController : HahaController
{
    private readonly IStockRepository _stockRepository;

    public StockController(IStockRepository stockRepository)
    {
        _stockRepository = stockRepository;
    }

    [HttpPost("inquiry/item")]
    public async Task<IActionResult> InquiryByItem([FromBody] RequestParameter parameter)
    {
        var results = await _stockRepository.InquiryByItem(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("inquiry/area")]
    public async Task<IActionResult> InquiryByArea([FromBody] RequestParameter parameter)
    {
        var results = await _stockRepository.InquiryByArea(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("inquiry/category")]
    public async Task<IActionResult> InquiryByCategory([FromBody] RequestParameter parameter)
    {
        var results = await _stockRepository.InquiryByCategory(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("inquiry/detail")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _stockRepository.InquiryDetail(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("ddl-lot-no-search")]
    public async Task<IActionResult> DDLLotSearch(string keyword, string ids, string warehouse, string area, string address, string item)
    {
        var results = await _stockRepository.DDLLotNo(keyword, warehouse, area, address, item);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.LotNo)).ToList();
        }

        return Success(results);
    }

    [HttpGet("ddl-lot-no-search-by-stock")]
    public async Task<IActionResult> DDLLotSearchByStock(string keyword, string ids, string warehouse, string area, string address, string item, string category)
    {
        var results = await _stockRepository.DDLLotNoByStock(keyword, warehouse, area, address, item, category);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.LotNo)).ToList();
        }

        return Success(results);
    }
}
