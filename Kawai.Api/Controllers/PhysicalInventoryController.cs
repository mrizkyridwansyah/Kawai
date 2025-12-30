using Kawai.Domain.DTOs;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Route("api/physical-inventory")]
[ApiController]
public class PhysicalInventoryController : HahaController
{
    private readonly IPhysicalInventoryRepository _physicalInventoryRepository;
    private readonly DataLogger _logger;
    public PhysicalInventoryController(IPhysicalInventoryRepository physicalInventoryRepository, DataLogger logger)
    {
        _physicalInventoryRepository = physicalInventoryRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> GetList([FromBody] RequestParameter parameter)
    {
        var results = await _physicalInventoryRepository.GetList(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] PhysicalInventoryUpdateDto model)
    {
        var before = await _physicalInventoryRepository.Capture(model.WarehouseCode, model.ProductCode);

        await _physicalInventoryRepository.Update(model, Auth.User.UserID);

        var after = await _physicalInventoryRepository.Capture(model.WarehouseCode, model.ProductCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Physical Inventory",
            EntityId = model.WarehouseCode,
            ReferenceId = model.ProductCode,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });

        return Success(model);
    }
}
