using Kawai.Domain.DTOs;

namespace Kawai.Domain.Interfaces;

public interface IManufactureLineRepository
{
    Task<List<ManufactureLineDto>> GetManufactureDDL(string keyword);
    Task<List<ManufactureLineDto>> GetLineDDL(string keyword, string manufactureCode);
}
