using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IProductionUnscheduleRepository
{

    //ddl yang line_unschedule =1 di manufacture_line
    Task<List<ManufactureLineDto>> GetLineUnscheduleDDL(string keyword);
    
    //ddl kondisi skrg hardcode di sp  item hanya untuk trade KMK 
    Task<List<ParentItemUnscheduleDto>> GetParentItemUnscheduleDDL(string keyword);

    Task<List<ProductionUnscheduleBOMDto>> GetBOMRequirement(RequestParameter payload);
    Task<List<ProductionUnscheduleResultDto>> GetListResults(RequestParameter payload);
    Task<List<ProductionUnscheduleDetailDto>> GetListResultsDetail(string id); //string ProdResultID);

    Task Save(ProductionUnscheduleModel model, string userId);
    //Task<Dictionary<string, object>> Capture(long productionId);
    
}
