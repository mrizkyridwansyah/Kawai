using Kawai.Domain.DTOs;

namespace Kawai.Domain.Interfaces;

public interface IFactoryRepository
{
    Task<List<FactoryDto>> GetDDL(string keyword);
    Task<List<FactoryDto>> GetDDLPrivileges(string keyword, string userId);
    Task<List<FactoryPrivilegesDto>> GetAllFactoryIncludePrivileges(string userId);

}
