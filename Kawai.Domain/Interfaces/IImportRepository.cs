using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IImportRepository
{
    Task<List<IDictionary<string, object>>> GetReferenceSheet(string sp);
    Task<List<ImportHistoryDto>> GetImportHistories(RequestParameter parameter);
    Task<ImportHistoryDto> GetDataImportHistory(string id);
    Task SaveHistory(ImportHistory history);
}
