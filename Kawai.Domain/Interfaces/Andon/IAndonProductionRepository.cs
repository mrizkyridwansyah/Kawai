using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IAndonProductionRepository
{

    #region ANDON
    Task<List<AndonProductionDto>> GetHeaderInfo(string Line, string Model, string Scheduledate);
    Task<List<AndonProductionDto>> GetInfoSchedule(string Line, string Model, string Scheduledate);
    Task<List<AndonProductionDto>> GetInfoTrolley(string Line, string Model, string Scheduledate);


    #endregion

}
