using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IAddressRepository
{
    Task<List<AddressDto>> GetAll(RequestParameter param);
    Task<List<AddressDto>> GetDDL(string keyword, string warehouse, string area);
    Task<List<AddressDto>> DDLSearchByStock(string keyword, string warehouse, string area, string item);
    Task<AddressDto> GetData(string areaCode);
    Task Create(Address address, string userId);
    Task Update(Address address, string userId);
    Task Remove(string addressCode, string userId);
    Task<Dictionary<string, object>> Capture(string addressCode);
    Task<List<AddressPrivilegesDto>> GetAllAddressIncludePrivileges(string userId);
}
