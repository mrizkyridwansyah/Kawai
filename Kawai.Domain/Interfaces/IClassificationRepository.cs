using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IClassificationRepository
{
    Task<List<ClassificationDto>> GetAll(RequestParameter param);
    Task<List<ClassificationTableDto>> GetListTableDetail(string tablename);
    Task<ClassificationTableDto> GetData(string code , String tablename);
    Task Create(Classification classification, string userId);
    Task Update(Classification classification, string userId);
    Task Remove(string code , string tablename, string userId);
    Task<Dictionary<string, object>> Capture(string code ,string tablename);



}
