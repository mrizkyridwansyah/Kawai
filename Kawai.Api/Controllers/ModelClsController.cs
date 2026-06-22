using Kawai.Api.Models;
using Kawai.Api.Services;
using Kawai.Data;
using Kawai.Domain.DTOs.Log;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Kawai.Api.Controllers;

[Authorize]
[Route("api/modelcls")]
[ApiController]
public class ModelClsController : HahaController
{
    private readonly IModelClsRepository _modelclsRepository;
    private readonly DataLogger _logger;

    public ModelClsController(IModelClsRepository modelclsRepository , DataLogger logger)
    {
        _modelclsRepository = modelclsRepository;
       
        _logger = logger;
    }

    [HttpPost("list")]
    public async Task<IActionResult> List([FromBody] RequestParameter parameter)
    {
        var results = await _modelclsRepository.GetAll(parameter);
        return DataTableResult(parameter, results);
    }

    
    [HttpGet("detail")]
    public async Task<IActionResult> Get(string id)
    {
        var result = await _modelclsRepository.GetData(id);
       
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

    
    [HttpPatch("update")]
    public async Task<IActionResult> Update([FromForm] ModelCls model)
    {
        var before = await _modelclsRepository.Capture(model.Model_Cls);

      
        if (model.ImageAttachment != null)
        {
            model.ImageName = String.IsNullOrEmpty(model.ImageName) ? Guid.NewGuid().UniqueId(30) : model.ImageName;
            FileStorage.SaveToImages(model.ImageName, model.ImageAttachment);
        }

        await _modelclsRepository.Update(model.Model_Cls, model, Auth.User.UserID);

        var after = await _modelclsRepository.Capture(model.Model_Cls);

        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Model Cls",
            EntityId = model.Model_Cls,
            ReferenceId = model.Model_Cls,
            Action = DataLogAction.Update,
            Before = before,
            After = after
        });
        return Success(after);
    }

    [HttpDelete("remove")]
    public async Task<IActionResult> Remove(string id)
    {
        var before = await _modelclsRepository.Capture(id);
        await _modelclsRepository.Remove(id, Auth.User.UserID);
        await _logger.SaveDataLog(new DataLogDto
        {
            DocumentType = "Model Cls",
            EntityId = id,
            ReferenceId = id,
            Action = DataLogAction.Delete,
            Before = before
        });

        return Success(before);
    }

    
     
}
