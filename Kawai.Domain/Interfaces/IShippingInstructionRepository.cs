using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IShippingInstructionRepository
{
    Task<List<ShippingInstructionDetailDto>> GetListDetail(RequestParameter parameter);
    Task<List<ShippingInstructionPickingDto>> GetListPicking(RequestParameter parameter);
    Task<ShippingInstructionDto> GetDataHeader(string shippinginstructionno);
    Task Create(ShippingInstruction si, string userId);
    Task Update(ShippingInstruction si, string userId);
    Task SavePicking(ShippingInstructionPicking picking, string userId);
    Task Remove(string shippinginstructionno);
    Task<List<ShippingInstructionFilterDto>> GetPODDL(string keyword,   string supplier, string typeDate, DateTime? periodFrom, DateTime? periodUntil, bool showOptionAll, string userId);
    Task<List<ShippingInstructionFilterDto>> GetSIDDL(string keyword,   string supplier, DateTime? periodFrom, DateTime? periodUntil,  string sourceMenu, string userId);

    Task<Dictionary<string, object>> Capture(string shippinginstructionno);
    Task<List<ShippingInstructionReportDto>> GetListReport(string sino);
}
