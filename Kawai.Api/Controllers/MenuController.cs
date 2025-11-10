using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/menu")]
[ApiController]
public class MenuController : HahaController
{
    private readonly IMenuRepository _menuRepository;
    private readonly IFactoryRepository _factoryRepository;
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IAreaRepository _areaRepository;
    private readonly DataLogger _logger;

    public MenuController(IMenuRepository menuRepository, IFactoryRepository factoryRepository, IWarehouseRepository warehouseRepository, IAreaRepository areaRepository, DataLogger logger)
    {
        _menuRepository = menuRepository;
        _factoryRepository = factoryRepository;
        _warehouseRepository = warehouseRepository;
        _areaRepository = areaRepository;
        _logger = logger;
    }

    [HttpGet("listprivileges")]
    public async Task<IActionResult> ListPrivileges(string userID)
    {
        var menuPriv = await _menuRepository.GetAllMenuIncludePrivileges(userID);
        var menuMobilePriv = await _menuRepository.GetAllMenuMobileIncludePrivileges(userID);
        var factoryPriv = await _factoryRepository.GetAllFactoryIncludePrivileges(userID);
        var warehousePriv = await _warehouseRepository.GetAllWarehouseIncludePrivileges(userID);
        var areaPriv = await _areaRepository.GetAllAreaIncludePrivileges(userID);
        var result = new
        {
            UserID = userID,
            MenuPrivileges = menuPriv,
            MenuMobilePrivileges = menuMobilePriv,
            FactoryPrivileges = factoryPriv,
            WarehousePrivileges = warehousePriv,
            AreaPrivileges = areaPriv
        };

        return Success(result);
    }

    [HttpGet("privileges")]
    public async Task<IActionResult> Privileges()
    {
        var result = await _menuRepository.GetUserMenuPrivileges(Auth.User.UserID);
        return Success(result);
    }

    [HttpGet("mobile/privileges")]
    public async Task<IActionResult> MobilePrivileges()
    {
        var result = await _menuRepository.GetUserMenuMobilePrivileges(Auth.User.UserID);
        return Success(result);
    }

    [HttpPost("privileges/save")]
    public async Task<IActionResult> SavePrivileges(Privileges model)
    {
        var before = await _menuRepository.Capture(model.UserId);
        await _menuRepository.SavePrivileges(Auth.User.UserID, model);
        var after = await _menuRepository.Capture(model.UserId);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Privileges",
            EntityId = model.UserId,
            ReferenceId = model.UserId,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

}
