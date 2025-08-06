using Kawai.Api.Models;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/user")]
[ApiController]
public class UserController : HahaController
{
    private readonly IUserRepository _userRepository;
    private readonly IMenuRepository _menuRepository;
    private readonly DataLogger _logger;

    public UserController(IUserRepository userRepository, IMenuRepository menuRepository, DataLogger logger)
    {
        _userRepository = userRepository;
        _menuRepository = menuRepository;
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _userRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string ids)
    {
        var results = await _userRepository.GetDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.UserID)).ToList();
        }

        return Success(results);
    }

    [HttpGet("jobddlsearch")]
    public async Task<IActionResult> JobDDLSearch(string keyword, string ids)
    {
        var results = await _userRepository.GetJobPositionDDL(keyword);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.JobPositionCode)).ToList();
        }

        return Success(results);
    }


    [HttpGet("user-info")]
    public async Task<IActionResult> UserInfo()
    {
        var result = await _menuRepository.GetUserMenuPrivileges(Auth.User.UserID);
        return Success(new
        {
            UserId = Auth.User.UserID,
            Auth.User.FullName,
            StatusAdmin = Auth.User.IsAdmin,
            Privileges = result
        });
    }

    [HttpGet("detail")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _userRepository.GetData(id);
        result.Password = result.Password.Decrypt(result.UserID.ToUpper());

        if (!String.IsNullOrEmpty(result.ImageName))
        {
            Stream? image = FileStorage.GetFromImages(result.ImageName);
            byte[] imageByte = null;

            if (image != null)
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    image.CopyTo(memoryStream);
                    imageByte = memoryStream.ToArray();
                }
                image.Dispose();
            }

            result.ImageBase64 = imageByte;
        }

        return Success(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromForm] User model)
    {
        model.Password = model.Password.Encrypt(model.UserID.ToUpper());

        if (model.ImageAttachment != null)
        {
            model.ImageName = Guid.NewGuid().UniqueId(30);
            FileStorage.SaveToImages(model.ImageName, model.ImageAttachment);
        }

        await _userRepository.Create(model, Auth.User.UserID);

        var after = await _userRepository.Capture(model.UserID);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master User",
            EntityId = model.UserID,
            ReferenceId = model.UserID,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromForm] User model)
    {
        var before = await _userRepository.Capture(model.UserID);

        model.Password = model.Password.Encrypt(model.UserID.ToUpper());

        if (model.ImageAttachment != null)
        {
            model.ImageName = String.IsNullOrEmpty(model.ImageName) ? Guid.NewGuid().UniqueId(30) : model.ImageName;
            FileStorage.SaveToImages(model.ImageName, model.ImageAttachment);
        }

        await _userRepository.Update(model.UserID, model, Auth.User.UserID);

        var after = await _userRepository.Capture(model.UserID);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master User",
            EntityId = model.UserID,
            ReferenceId = model.UserID,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string id)
    {
        var before = await _userRepository.Capture(id);
        await _userRepository.Remove(id, Auth.User.UserID);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master User",
            EntityId = id,
            ReferenceId = id,
            Action = DataLogAction.Delete,
            Before = before
        });

        return Success(before);
    }

    [HttpPatch("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] UserChangePassword model)
    {
        var before = await _userRepository.Capture(Auth.User.UserID);

        Dictionary<string, List<string>> Errors = [];
        if (string.Compare(model.Password.Encrypt(Auth.User.UserID.ToUpper()), before["Password"].ToString(), StringComparison.OrdinalIgnoreCase) != 0)
            AddError(Errors, "Password", "Password saat ini salah!");

        if (Errors.Any()) return Invalid(Errors);

        model.NewPassword = model.NewPassword.Encrypt(Auth.User.UserID.ToUpper());

        await _userRepository.ChangePassword(Auth.User.UserID, model.NewPassword);

        var after = await _userRepository.Capture(Auth.User.UserID);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Master User",
            EntityId = Auth.User.UserID,
            ReferenceId = Auth.User.UserID,
            Action = DataLogAction.Update,
            Activity = "Change Password",
            Before = before,
            After = after
        });
        return Success(after);
    }
}
