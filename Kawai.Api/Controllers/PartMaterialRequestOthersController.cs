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
[Route("api/partmaterialrequestothers")]
[ApiController]
public class PartMaterialRequestOthersController : HahaController
{
    private readonly IPartMaterialRequestOthersRepository _partmaterialrequestothersRepository;
    private readonly DataLogger _logger;

    public PartMaterialRequestOthersController(IPartMaterialRequestOthersRepository partmaterialrequestothersRepository, DataLogger logger)
    {
        _partmaterialrequestothersRepository = partmaterialrequestothersRepository;
        _logger = logger;
    }


    [HttpPost("list-history")]
    public async Task<IActionResult> ListHistory([FromBody] RequestParameter parameter)
    {
        var results = await _partmaterialrequestothersRepository.GetListHistory(parameter);
        return DataTableResult(parameter, results);
    }


    [HttpGet("data-header")]
    public async Task<IActionResult> GetDataHeader(long requestid)
    {
        var result = await _partmaterialrequestothersRepository.GetDataHeader(requestid);
        return Success(result);
    }

    [HttpPost("list-scan")]
    public async Task<IActionResult> ListScan([FromBody] RequestParameter parameter)
    {
        var results = await _partmaterialrequestothersRepository.GetListScan(parameter);
        return DataTableResult(parameter, results);
    }

    [HttpPost("list-request-detail")]
    public async Task<IActionResult> GetListRequestDetail([FromBody] RequestParameter parameter)
    {
        var results = await _partmaterialrequestothersRepository.GetListRequestDetail(parameter);
        return DataTableResult(parameter, results);
    }

   [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] PartMaterialRequestOthers model)
    {
        await _partmaterialrequestothersRepository.Create(model, Auth.User.UserID);

        var after = await _partmaterialrequestothersRepository.Capture(model.RequestId.Value);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Material Request Others Create",
            EntityId = model.RequestId.ToString(),
            ReferenceId = model.RequestNo,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] PartMaterialRequestOthers model)
    {
        var before = await _partmaterialrequestothersRepository.Capture(model.RequestId.Value);

        await _partmaterialrequestothersRepository.Update(model, Auth.User.UserID);

        var after = await _partmaterialrequestothersRepository.Capture(model.RequestId.Value);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Part Material Request Others",
            EntityId = model.RequestId.ToString(),
            ReferenceId = model.RequestNo,
            Before = before,
            After = after,
            Action = DataLogAction.Update
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(long requestid)
    {
        var before = await _partmaterialrequestothersRepository.Capture(requestid);

        await _partmaterialrequestothersRepository.Remove(requestid);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Delete Part Material Request Others",
            EntityId = requestid.ToString(),
            ReferenceId = requestid.ToString(),
            Before = before,
            After = null,
            Action = DataLogAction.Delete,
            Activity = "Delete Part Material Request Others"
        });

        return Success();
    }


    [HttpGet("ddlsearch")]
    public async Task<IActionResult> DDLSearch(string keyword, string status,   string ids)
    {
        var results = await _partmaterialrequestothersRepository.DDLSearch(keyword, status,  Auth.User.UserID);
        if (!string.IsNullOrEmpty(ids))
        {
            var idList = ids.Split(',').Select(id => id.Trim()).ToList();
            results = results.Where(x => idList.Contains(x.RequestID.ToString())).ToList();
        }

        return Success(results.Take(100));
    }
 
   

}
