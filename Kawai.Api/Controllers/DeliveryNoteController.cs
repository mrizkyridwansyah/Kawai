using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/delivery-note")]
[ApiController]
public class DeliveryNoteController : HahaController
{
    private readonly IDeliveryNoteRepository _deliveryNoteRepository;
    private readonly DataLogger _logger;

    public DeliveryNoteController(IDeliveryNoteRepository deliveryNoteRepository, DataLogger logger)
    {
        _deliveryNoteRepository = deliveryNoteRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] RequestParameter parameter)
    {
        var results = await _deliveryNoteRepository.GetList(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> GetDetail(string dnNumber)
    {
        var result = await _deliveryNoteRepository.GetDetail(dnNumber);
        return Success(result);
    }

    [HttpGet("list-detail")]
    public async Task<IActionResult> GetListDetail(string dnNumber)
    {
        var results = await _deliveryNoteRepository.GetListDetail(dnNumber);
        return Success(results);
    }
}
