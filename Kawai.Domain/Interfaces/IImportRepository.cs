using Kawai.Domain.Models;

namespace Kawai.Domain.Interfaces;

public interface IImportRepository
{
    Task<List<IDictionary<string, object>>> GetReferenceSheet(string sp);
    Task SaveHistory(ImportHistory history);
}
