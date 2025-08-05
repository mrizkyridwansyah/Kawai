using Kawai.Domain.DTOs;
using Kawai.Domain.Models;

namespace Kawai.Domain.Interfaces;

public interface IMenuRepository
{
    Task<List<MenuDto>> GetAllMenuIncludePrivileges(string userId);
    Task<List<MenuDto>> GetUserMenuPrivileges(string userId);
    Task SavePrivileges(string userId, Privileges privileges);
    Task<Dictionary<string, object>> Capture(string userId);

}
