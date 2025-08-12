using Kawai.Domain.DTOs;

namespace Kawai.Domain.Interfaces;

public interface IHSRepository
{
    Task<List<HSDto>> GetDDL(string keyword);
}
