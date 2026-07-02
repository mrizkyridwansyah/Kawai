using Kawai.Domain.DTOs;
using Kawai.Domain.Models;

namespace Kawai.Domain.Interfaces;

public interface IShippingInstructionRepository
{
    Task<List<ShippingInstructionFilterDto>> GetFilterDDL(string custCode, DateTime? dateFrom, DateTime? dateTo);
    Task<List<ShippingInstructionGridRowDto>> GetList(string poNo, bool isNew);
    Task Submit(List<ShippingInstructionRequest> requests, string userId);
    Task UpdatePicking(List<ShippingPickingRequest> requests, string userId);
    Task<List<NGClaimReportDto>> GetListReport(string sino);
}
