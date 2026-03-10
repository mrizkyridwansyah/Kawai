using Kawai.Data.SqlConnections;
using Kawai.Data;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;

namespace Kawai.Data.Repositories;

public class ShippingInstructionRepository : IShippingInstructionRepository
{
    private readonly DbExecutor _dbExecutor;

    public ShippingInstructionRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ShippingInstructionFilterDto>> GetFilterDDL(string custCode, DateTime? dateFrom, DateTime? dateTo)
    {
        var fromDate = dateFrom ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var toDate = dateTo ?? fromDate.AddMonths(1).AddDays(-1);
        var customer = string.IsNullOrWhiteSpace(custCode) || string.Equals(custCode, "ALL", StringComparison.OrdinalIgnoreCase)
            ? "All"
            : custCode;

        var data = (await _dbExecutor.QueryListAsync<ShippingInstructionFilterDto>(
            "sp_Wms_ShippingInstruction_Filter_DLL",
            new
            {
                CustCode = customer,
                DateFrom = fromDate,
                DateTo = toDate
            }
        )).ToList();

        foreach (var item in data)
            item.DDLDescription = item.SI_No;

        return data;
    }

    public async Task<List<ShippingInstructionGridRowDto>> GetList(string poNo, bool isNew)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_ShippingInstruction_GetList",
            new
            {
                PONo = string.IsNullOrWhiteSpace(poNo) ? null : poNo.Trim()
            },
            async multi =>
            {
                var details = (await multi.ReadAsync<dynamic>()).ToList();
                var serials = (await multi.ReadAsync<dynamic>()).ToList();
                return (details, serials);
            }
        );

        var detailRows = result.details
            .Select(MapDetail)
            .Where(x => !string.IsNullOrWhiteSpace(x.ItemCode) && x.SeqNo > 0)
            .ToList();

        var serialRows = result.serials
            .Select(MapSerial)
            .Where(x => !string.IsNullOrWhiteSpace(x.ItemCode) && x.SeqNo > 0)
            .ToList();

        foreach (var row in detailRows)
        {
            row.Serials = serialRows
                .Where(x => x.ItemCode == row.ItemCode && x.SeqNo == row.SeqNo)
                .OrderBy(x => x.SerialNo)
                .ToList();
        }

        return detailRows.OrderBy(x => x.SeqNo).ToList();
    }

    private static ShippingInstructionGridRowDto MapDetail(dynamic row)
    {
        var dict = ToDictionary(row);
        return new ShippingInstructionGridRowDto
        {
            IsPicking = GetInt(dict, "isPicking", "IsPicking") == 1,
            SINo = GetString(dict, "SI_NO", "SI_No", "SINo"),
            SIDate = GetDate(dict, "SI_Date", "SIDate"),
            PONo = GetString(dict, "PO_No", "PONo"),
            SeqNo = GetInt(dict, "PO_SeqNo", "Seq_No"),
            ItemCode = GetString(dict, "Item_Code"),
            PartNumber = GetString(dict, "Item_Code"),
            Description = GetString(dict, "Item_Name"),
            Unit = GetString(dict, "Unit_Desc"),
            QtyShipping = GetDecimal(dict, "Qty_Shipping", "Qty"),
            DeliveryDate = GetDate(dict, "PO_DelivDate", "Delivery_Date"),
            QtyStock = GetDecimal(dict, "Qty_Stock"),
            QtyPicking = GetDecimal(dict, "Qty_Picking"),
            SerialNoFrom = GetString(dict, "SerialNo_From", "SerialNoFrom"),
            SerialNoTo = GetString(dict, "SerialNo_To", "SerialNoto"),
        };
    }

    public async Task Submit(List<ShippingInstructionRequest> requests, string userId)
    {
        var validRequests = (requests ?? [])
            .Where(x => !string.IsNullOrWhiteSpace(x.PONo)
                && x.POSeqNo > 0
                && !string.IsNullOrWhiteSpace(x.ItemCode)
                && x.SIDate != default)
            .ToList();

        if (!validRequests.Any())
            return;

        await _dbExecutor.ExecuteAsync(
            "sp_Wms_ShippingInstruction_Insert",
            new
            {
                LastUser = userId,
                Request = DataTableHelper.ToDataTable(validRequests)
            }
        );
    }

    public async Task UpdatePicking(List<ShippingPickingRequest> requests, string userId)
    {
        var validRequests = (requests ?? [])
            .Where(x => !string.IsNullOrWhiteSpace(x.PONo)
                && x.POSeqNo > 0
                && !string.IsNullOrWhiteSpace(x.ItemCode)
                && !string.IsNullOrWhiteSpace(x.SerialNo))
            .ToList();

        if (!validRequests.Any())
            return;

        await _dbExecutor.ExecuteAsync(
            "sp_Wms_ShippingPicking_Update",
            new
            {
                LastUser = userId,
                Request = DataTableHelper.ToDataTable(validRequests)
            }
        );
    }

    private static ShippingInstructionGridSerialDto MapSerial(dynamic row)
    {
        var dict = ToDictionary(row);
        return new ShippingInstructionGridSerialDto
        {
            PONo = GetString(dict, "PO_No", "PONo"),
            ItemCode = GetString(dict, "Item_Code"),
            SeqNo = GetInt(dict, "PO_SeqNo", "Seq_No"),
            SerialNo = GetString(dict, "Serial_No"),
            IsPicking = GetInt(dict, "IsPicking", "isPicking") == 1,
            Address = GetString(dict, "Address", "Addres"),
            PickingDate = GetDate(dict, "Picking_Date"),
            PickingTime = GetString(dict, "Picking_Time"),
            PickingBy = GetString(dict, "Picking_By"),
        };
    }

    private static Dictionary<string, object> ToDictionary(dynamic row)
    {
        return ((IDictionary<string, object>)row)
            .ToDictionary(x => x.Key, x => x.Value, StringComparer.OrdinalIgnoreCase);
    }

    private static object GetValue(Dictionary<string, object> dict, params string[] keys)
    {
        foreach (var key in keys)
        {
            if (dict.TryGetValue(key, out var value) && value != null && value != DBNull.Value)
                return value;
        }
        return null;
    }

    private static string GetString(Dictionary<string, object> dict, params string[] keys)
    {
        return Convert.ToString(GetValue(dict, keys)) ?? string.Empty;
    }

    private static int GetInt(Dictionary<string, object> dict, params string[] keys)
    {
        var value = GetValue(dict, keys);
        if (value == null) return 0;
        return int.TryParse(Convert.ToString(value), out var result) ? result : 0;
    }

    private static decimal GetDecimal(Dictionary<string, object> dict, params string[] keys)
    {
        var value = GetValue(dict, keys);
        if (value == null) return 0m;
        return decimal.TryParse(Convert.ToString(value), out var result) ? result : 0m;
    }

    private static DateTime? GetDate(Dictionary<string, object> dict, params string[] keys)
    {
        var value = GetValue(dict, keys);
        if (value == null) return null;
        return DateTime.TryParse(Convert.ToString(value), out var result) ? result : null;
    }
}
