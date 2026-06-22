using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IAndonFilterRepository
{

    #region ANDON
    Task<List<AndonFilterDto>> DDLArea(string keyword, string warehouseCode);
    Task<List<AndonFilterDto>> DDLLine(string keyword);
    Task<List<AndonFilterDto>> DDLModel(string keyword);
    #endregion

}
