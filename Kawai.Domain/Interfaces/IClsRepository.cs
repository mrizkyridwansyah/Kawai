using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IClsRepository
{
     Task<List<ClsDto>> GetDDL(  string keyword ,string typedata);
  


}
