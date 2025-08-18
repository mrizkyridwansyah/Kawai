using ClosedXML.Excel;
using DocumentFormat.OpenXml.EMMA;
using Kawai.Api.Services;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/item-packing-supplier")]
[ApiController]
public class ItemPackingSupplierController : HahaController
{
    private readonly IItemPackingSupplierRepository _itemPackingSupplierRepository;
    private readonly DataLogger _logger;

    public ItemPackingSupplierController(IItemPackingSupplierRepository itemPackingSupplierRepository, DataLogger logger)
    {
        _itemPackingSupplierRepository = itemPackingSupplierRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _itemPackingSupplierRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> Get(string supplierCode, string itemCode)
    {
        var result = await _itemPackingSupplierRepository.GetData(supplierCode, itemCode);
        return Success(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] ItemPackingSupplier model)
    {
        await _itemPackingSupplierRepository.Create(model, Auth.User.UserID);

        var after = await _itemPackingSupplierRepository.Capture(model.SupplierCode, model.ItemCode);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Item Packing Supplier",
            EntityId = model.SupplierCode + "|" + model.ItemCode,
            ReferenceId = model.SupplierCode + "|" + model.ItemCode,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] ItemPackingSupplier model)
    {
        var before = await _itemPackingSupplierRepository.Capture(model.SupplierCode, model.ItemCode);
        await _itemPackingSupplierRepository.Update(model, Auth.User.UserID);
        var after = await _itemPackingSupplierRepository.Capture(model.SupplierCode, model.ItemCode);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Item Packing Supplier",
            EntityId = model.SupplierCode + "|" + model.ItemCode,
            ReferenceId = model.SupplierCode + "|" + model.ItemCode,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string supplierCode, string itemCode)
    {
        var before = await _itemPackingSupplierRepository.Capture(supplierCode, itemCode);
        await _itemPackingSupplierRepository.Remove(supplierCode, itemCode);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Item Packing Supplier",
            EntityId = supplierCode + "|" + itemCode,
            ReferenceId = supplierCode + "|" + itemCode,
            Action = DataLogAction.Delete,
            Before = before
        });

        return Success(before);
    }
}
