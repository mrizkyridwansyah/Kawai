using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/itemsetting")]
[ApiController]
public class ItemSettingController : HahaController
{
    private readonly IItemSettingRepository _itemsettingRepository;
  
    private readonly DataLogger _logger;

    public ItemSettingController(IItemSettingRepository itemsettingRepository, DataLogger logger)
    {
        _itemsettingRepository = itemsettingRepository;
        _logger = logger;
        
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _itemsettingRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    
        
    
    [HttpPost("save")]
    public async Task<IActionResult> Save(ItemSetting model)
    {
        var before = await _itemsettingRepository.Capture(model.Model_Cls , model.ParentItem_Code);
        await _itemsettingRepository.SaveItemSetting(model,Auth.User.UserID);
        var after = await _itemsettingRepository.Capture(model.Model_Cls, model.ParentItem_Code);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "ItemSetting",
            EntityId = model.Model_Cls,
            ReferenceId = model.ParentItem_Code,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    

}
