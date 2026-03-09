using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/classification")]
[ApiController]
public class ClassificationController : HahaController
{
    private readonly IClassificationRepository _classificationRepository;
    private readonly DataLogger _logger;

    public ClassificationController(IClassificationRepository classificationRepository, DataLogger logger)
    {
        _classificationRepository = classificationRepository;
        _logger = logger;
    }

    [HttpPost("listtab")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _classificationRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }


    [HttpGet("listdetail")]
    public async Task<IActionResult> ListDetail(string tablename)
    {
        var tableList = await _classificationRepository.GetListTableDetail(tablename);

        var result = new
        {
            TableName = tablename,
            TableData = tableList
        };

        return Success(result);
    }

    [HttpGet("detail")]
    public async Task<IActionResult> Get(string id ,string tableName)
    {
        var result = await _classificationRepository.GetData(id , tableName);
        return Success(result);
    }

    [HttpPost("create")]
    public async Task<IActionResult> Create([FromBody] Classification model)
    {
        await _classificationRepository.Create(model, Auth.User.UserID);

        var after = await _classificationRepository.Capture(model.Code, model.TableName);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Cls Master",
            EntityId = model.TableName,
            ReferenceId = model.Code,
            Before = null,
            After = after,
            Action = DataLogAction.Create
        });

        return Success(after);
    }

    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromBody] Classification model)
    {
        var before = await _classificationRepository.Capture(model.Code, model.TableName);
        await _classificationRepository.Update(model, Auth.User.UserID);
        var after = await _classificationRepository.Capture(model.Code, model.TableName);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Cls Master",
            EntityId = model.TableName,
            ReferenceId = model.Code,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string id, string tableName)
    {
        var before = await _classificationRepository.Capture(id, tableName);
        await _classificationRepository.Remove(id, tableName, Auth.User.UserID);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Cls Master",
            EntityId = tableName,
            ReferenceId = id,
            Action = DataLogAction.Delete,
            Before = before
        });

        return Success(before);
    }




}
