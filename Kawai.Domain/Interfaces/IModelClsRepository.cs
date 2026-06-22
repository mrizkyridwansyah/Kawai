using Kawai.Api.DTOs;
using Kawai.Api.Models;
using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;
using System.Data;

namespace Kawai.Domain.Interfaces;

public interface IModelClsRepository
{
    Task<List<ModelClsDto>> GetAll(RequestParameter param);
    Task Update(string model_cls, ModelCls modelcls, string updateUser);
    Task Remove(string model_cls, string deleteUser);
    Task<Dictionary<string, object>> Capture(string model_cls);
    Task<ModelClsDto> GetData(string model_cls);


}
