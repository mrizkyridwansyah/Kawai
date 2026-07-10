using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IManufactureLineRepository
{
    Task<List<ManufactureLineDto>> GetManufactureDDL(string keyword);
    Task<List<ManufactureLineDto>> GetLineDDL(string keyword, string manufactureCode);

    Task<List<ManufactureLineDto>> GetAll(RequestParameter param);
    Task<ManufactureLineDto> GetData(string warehouseCode);
    Task Update(Manufactureline manufactureline, string userId);
    Task<Dictionary<string, object>> Capture(string linecode);

}
