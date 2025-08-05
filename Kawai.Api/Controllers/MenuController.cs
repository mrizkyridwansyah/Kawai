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
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly DataLogger _logger;

    public MenuController(IMenuRepository menuRepository, IWarehouseRepository warehouseRepository, DataLogger logger)
    {
        _menuRepository = menuRepository;
        _warehouseRepository = warehouseRepository;
        _logger = logger;
    }

    [HttpGet("listprivileges")]
    public async Task<IActionResult> ListPrivileges(string userID)
    {
        var menuPriv = await _menuRepository.GetAllMenuIncludePrivileges(userID);
        var warehousePriv = await _warehouseRepository.GetAllWarehouseIncludePrivileges(userID);
        var result = new
        {
            UserID = userID,
            MenuPrivileges = menuPriv,
            WarehousePrivileges = warehousePriv
        };

        return Success(result);
    }

    [HttpGet("privileges")]
    public async Task<IActionResult> Privileges()
    {
        var result = await _menuRepository.GetUserMenuPrivileges(Auth.User.UserID);
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
