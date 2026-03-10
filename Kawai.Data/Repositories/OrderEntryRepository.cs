using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;

namespace Kawai.Data.Repositories;

public class OrderEntryRepository : IOrderEntryRepository
{
    private readonly DbExecutor _dbExecutor;

    public OrderEntryRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<OrderEntryFilterDto>> GetFilterDDL(string custCode, DateTime? dateFrom, DateTime? dateTo)
    {
        var fromDate = dateFrom ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var toDate = dateTo ?? fromDate.AddMonths(1).AddDays(-1);
        var customer = string.IsNullOrWhiteSpace(custCode) || string.Equals(custCode, "ALL", StringComparison.OrdinalIgnoreCase)
            ? "All"
            : custCode;

        var data = (await _dbExecutor.QueryListAsync<OrderEntryFilterDto>(
            "sp_Wms_OrderEntry_Filter_DLL",
            new
            {
                CustCode = customer,
                DateFrom = fromDate,
                DateTo = toDate
            }
        )).ToList();

        foreach (var item in data)
            item.DDLDescription = item.PO_No;

        return data;
    }
}
