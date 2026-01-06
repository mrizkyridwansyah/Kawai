using ClosedXML.Excel;
using Kawai.Api.Services;
using Kawai.Data.Repositories;
using Kawai.Domain;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/periodsetting")]
[ApiController]
public class PeriodSettingController : HahaController
{
    private readonly IPeriodSettingRepository _periodsettingRepository;
    private readonly DataLogger _logger;

    public PeriodSettingController(IPeriodSettingRepository periodsettingRepository, DataLogger logger)
    {
        _periodsettingRepository = periodsettingRepository;
        _logger = logger;
    }

    

    [HttpPost("listdetail")]
    public async Task<IActionResult> GetListDetail([FromBody] RequestParameter parameter)
    {
        var results = await _periodsettingRepository.GetListDetail(parameter);
        return DataTableResult(parameter, results);
    }

    
 
    [HttpPatch("save")]
    public async Task<IActionResult> Update([FromBody] PeriodSetting model)
    {
        var before = await _periodsettingRepository.Capture(model.Year);

        await _periodsettingRepository.Update(model, Auth.User.UserID);

        var after = await _periodsettingRepository.Capture(model.Year);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Period Setting",
            EntityId = model.Year.ToString(),
            ReferenceId = model.Year.ToString(),
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });
        return Success(after);
    }

   
    


}
