using Kawai.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/grouping-class-part")]
[ApiController]
public class GroupingClassPartController : HahaController
{
    private readonly IClsRepository _clsRepository;

    public GroupingClassPartController(IClsRepository clsRepository)
    {
        _clsRepository = clsRepository;
    }
   
    [HttpGet("ddlsearch-privileges")]
    public async Task<IActionResult> DDLSearch(string keyword, string ids)
    {
        var results = await _clsRepository.GetGroupingClassPartDDLPrivileges(keyword, Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.GroupingClassPartCode)).ToList();
        }

        return Success(results);
    }
}
