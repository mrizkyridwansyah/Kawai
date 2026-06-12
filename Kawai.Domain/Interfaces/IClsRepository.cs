using Kawai.Domain.DTOs;

namespace Kawai.Domain.Interfaces;

public interface IClsRepository
{
    Task<List<ClsDto>> GetDDL(string keyword, string typedata);
    Task<List<GroupingClassPartDto>> GetGroupingClassPartDDLPrivileges(  string keyword ,string userId);
}
