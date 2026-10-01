using Dapper;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs.RackDashboard;
using Kawai.Domain.Interfaces;

namespace Kawai.Data.Repositories;

public class RackDashboardRepository : IRackDashboardRepository
{
    private readonly DbExecutor _dbExecutor;

    public RackDashboardRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<RackDashboardWarehouseDto>> GetWarehouses()
    {
        var result = await _dbExecutor.QueryListAsync<RackDashboardWarehouseDto>(
            StoreProcedures.RackDashboard.WarehouseList
        );
        return result.ToList();
    }

    public async Task<List<RackDashboardAreaDto>> GetAreas(string? warehouseCode)
    {
        var param = new 
        { 
            WarehouseCode = string.IsNullOrWhiteSpace(warehouseCode) ? null : warehouseCode.Trim() 
        };

        var result = await _dbExecutor.QueryListAsync<RackDashboardAreaDto>(
            StoreProcedures.RackDashboard.AreaList,
            param
        );
        return result.ToList();
    }

    public async Task<List<RackDashboardLocationDto>> GetLocations(string? warehouse, string? area, string? status, string? search, string? item = null)
    {
        var param = new
        {
            Warehouse = string.IsNullOrWhiteSpace(warehouse) ? null : warehouse.Trim(),
            Area = string.IsNullOrWhiteSpace(area) ? null : area.Trim(),
            Status = string.IsNullOrWhiteSpace(status) ? null : status.Trim().ToLowerInvariant(),
            Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
            ItemCode = string.IsNullOrWhiteSpace(item) ? null : item.Trim()
        };

        var result = await _dbExecutor.QueryListAsync<RackDashboardLocationDto>(
            StoreProcedures.RackDashboard.LocationList,
            param
        );
        return result.ToList();
    }

    public async Task<RackDashboardAddressDetailDto?> GetAddressDetail(string warehouseCode, string areaCode, string addressCode)
    {
        var param = new
        {
            WarehouseCode = warehouseCode.Trim(),
            AreaCode = areaCode.Trim(),
            AddressCode = addressCode.Trim()
        };

        return await _dbExecutor.QueryMultipleAsync<RackDashboardAddressDetailDto?>(
            StoreProcedures.RackDashboard.AddressDetail,
            param,
            async reader =>
            {
                var header = await reader.ReadFirstOrDefaultAsync<dynamic>();
                if (header == null) return null;

                var stocks = (await reader.ReadAsync<RackDashboardStockDetailItemDto>()).ToList();
                decimal totalQty = stocks.Sum(x => x.Qty);
                bool isRegistered = false;
                if (header.IsRegistered != null)
                {
                    isRegistered = Convert.ToInt32(header.IsRegistered) == 1;
                }
                string status = !isRegistered ? "unmapped" : (totalQty > 0 ? "occupied" : "empty");

                return new RackDashboardAddressDetailDto
                {
                    WarehouseCode = warehouseCode,
                    WarehouseName = header.WarehouseName ?? warehouseCode,
                    AreaCode = areaCode,
                    AreaName = header.AreaName ?? areaCode,
                    AddressCode = addressCode,
                    AddressName = header.AddressName ?? addressCode,
                    Status = status,
                    TotalQty = totalQty,
                    StockRowCount = stocks.Count,
                    ItemCount = stocks.Select(x => x.ItemCode).Where(x => !string.IsNullOrEmpty(x)).Distinct().Count(),
                    IsRegistered = isRegistered,
                    Stocks = stocks
                };
            }
        );
    }

    public async Task<RackDashboardKpiSummaryDto> GetKpi(string? warehouse, string? area, string? status, string? search, string? item = null)
    {
        var param = new
        {
            Warehouse = string.IsNullOrWhiteSpace(warehouse) ? null : warehouse.Trim(),
            Area = string.IsNullOrWhiteSpace(area) ? null : area.Trim(),
            Status = string.IsNullOrWhiteSpace(status) ? null : status.Trim().ToLowerInvariant(),
            Search = string.IsNullOrWhiteSpace(search) ? null : search.Trim(),
            ItemCode = string.IsNullOrWhiteSpace(item) ? null : item.Trim()
        };

        var result = await _dbExecutor.QueryMultipleAsync<RackDashboardKpiSummaryDto>(
            StoreProcedures.RackDashboard.KpiSummary,
            param,
            async reader =>
            {
                var summary = await reader.ReadFirstOrDefaultAsync<RackDashboardKpiSummaryDto>()
                              ?? new RackDashboardKpiSummaryDto();
                if (!reader.IsConsumed)
                {
                    var items = (await reader.ReadAsync<RackDashboardKpiItemDto>()).ToList();
                    summary.Items = items;
                    if (summary.TotalItemCount == 0 && items.Count > 0)
                    {
                        summary.TotalItemCount = items.Count;
                    }
                }
                return summary;
            }
        );

        return result ?? new RackDashboardKpiSummaryDto();
    }
}
