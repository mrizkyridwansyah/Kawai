using ClosedXML.Excel;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain;
using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/inventoryclosing")]
[ApiController]
public class InventoryClosingController : HahaController
{
    private readonly IInventoryClosingRepository _inventoryclosingRepository;
    private readonly DataLogger _logger;

    public InventoryClosingController(IInventoryClosingRepository inventoryclosingRepository, DataLogger logger)
    {
        _inventoryclosingRepository = inventoryclosingRepository;
        _logger = logger;
    }

    [HttpPost("getcurrent")]
    public async Task<IActionResult> GetCurrent()
    {
        var results = await _inventoryclosingRepository.GetCurrent();
        return Success(results);
    }

    [HttpPost("process")]
    public async Task<IActionResult> ProcessClosing([FromBody] ClosingRequestDto dto)
    {
        var userId = Auth.User.UserID;

        await _inventoryclosingRepository.ProcessClosing(
            dto.IvtYear,
            dto.IvtMonth,
            userId
        );

        return Ok(new { Success = true });
    }




}
