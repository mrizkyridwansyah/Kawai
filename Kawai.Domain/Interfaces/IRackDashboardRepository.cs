using Kawai.Domain.DTOs.RackDashboard;

namespace Kawai.Domain.Interfaces;

public interface IRackDashboardRepository
{
    Task<List<RackDashboardWarehouseDto>> GetWarehouses();
    Task<List<RackDashboardAreaDto>> GetAreas(string? warehouseCode);
    Task<List<RackDashboardLocationDto>> GetLocations(string? warehouse, string? area, string? status, string? search, string? item = null);
    Task<RackDashboardAddressDetailDto?> GetAddressDetail(string warehouseCode, string areaCode, string addressCode);
    Task<RackDashboardKpiSummaryDto> GetKpi(string? warehouse, string? area, string? status, string? search, string? item = null);
}
