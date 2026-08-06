using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;
using System.Data;

namespace Kawai.Data.Repositories;

public class ShippingInstructionRepository : IShippingInstructionRepository
{
    private readonly DbExecutor _dbExecutor;

    public ShippingInstructionRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<ShippingInstructionPickingDto>> GetListPicking(RequestParameter param)
    {
        string sp = "sp_Wms_Shipping_Instruction_List_Picking";
        return (await _dbExecutor.QueryListAsync<ShippingInstructionPickingDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task SavePicking(ShippingInstructionPicking model, string userId)
    {

        var header = model.Header.First();

        var commands = new List<(string, object?, CommandType)>();
        commands.Add((
            "sp_Wms_Shipping_Instruction_Save_Picking",
            new
            {
                header.ShippingInstructionNo,
                header.ItemCode,
                header.PONumber,
                header.PO_SeqNo,
                Details = DataTableHelper.ToDataTable(model.Details),
                PickingBy = userId
            },
            CommandType.StoredProcedure
        ));

        

        await _dbExecutor.ExecuteMultiCommandWithTransactionAsync(commands);
    }


    public async Task<List<ShippingInstructionDetailDto>> GetListDetail(RequestParameter param)
    {
        
        var paramSupplier = param.GetParam("Supplier");
        var paramPONumber = param.GetParam("PONumber");
        var paramShippingInstructionNo = param.GetParam("ShippingInstructionNo");
        var paramDeliveryFrom = param.GetParam("DeliveryFrom");
        var paramDeliveryTo = param.GetParam("DeliveryTo");
 
        string sp = "sp_Wms_Shipping_Instruction_ListDetail";
        return (await _dbExecutor.QueryListAsync<ShippingInstructionDetailDto>(sp, new
        {
                Supplier =  paramSupplier ,
                PONumber =  paramPONumber ,
                ShippingInstructionNo =  paramShippingInstructionNo ,
                DeliveryFrom =  paramDeliveryFrom ,
                DeliveryTo = paramDeliveryTo ,
 
        })).ToList();
    }

    public async Task<ShippingInstructionDto> GetDataHeader(string shippinginstructionno)
    {
        string sp = "sp_Wms_Shipping_Instruction_DataHeader";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ShippingInstructionDto>(sp, new { ShippingInstructionNo = shippinginstructionno });
    }

    public async Task<List<ShippingInstructionFilterDto>> GetSIDDL(string keyword,  string supplier, DateTime? periodFrom, DateTime? periodUntil,   string sourceMenu, string userId)
    {
        string sp = "sp_Wms_Shipping_Instruction_DDL";

        return (await _dbExecutor.QueryListAsync<ShippingInstructionFilterDto>(sp, new
        {
            Keyword = keyword ?? "",
            SourceMenu = sourceMenu ?? "",
            SupplierCode = String.IsNullOrEmpty(supplier) ? "ALL" : supplier,
            PeriodFrom = periodFrom,
            PeriodUntil = periodUntil,
            UserId = userId
        })).ToList();
    }

    public async Task<List<ShippingInstructionFilterDto>> GetPODDL(string keyword, string supplier, string typeDate, DateTime? periodFrom, DateTime? periodUntil, bool showOptionAll, string userId)
    {
        string sp = "sp_Wms_Shipping_Instruction_PODDL";
        var today = DateTime.Today;
        var awalBulan = new DateTime(today.Year, today.Month, 1);

        return (await _dbExecutor.QueryListAsync<ShippingInstructionFilterDto>(sp, new
        {
            Keyword = keyword ?? "",
            SupplierCode = String.IsNullOrEmpty(supplier) ? "ALL" : supplier,
            TypeDate = typeDate,
            PeriodFrom = periodFrom.HasValue ? periodFrom.Value : awalBulan,
            PeriodUntil = periodUntil.HasValue ? periodUntil.Value : DateTime.Today,
            ShowOptionAll = showOptionAll,
            UserId = userId
        })).ToList();
    }

    public async Task Create(ShippingInstruction si, string userId)
    {
        si.ShippingInstructionNo = await _dbExecutor.QuerySingleOrDefaultAsync<string>("sp_Wms_Shipping_Instruction_GenerateCode");
        var DetaiTest = DataTableHelper.ToDataTable(si.Details);
        string sqlHeader = "sp_Wms_Shipping_Instruction_Create";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            si.ShippingInstructionNo,
            si.ShippingInstructionDate,
            si.PONumber,
            si.Supplier,
            Details = DataTableHelper.ToDataTable(si.Details),
            RegisterBy = userId
        });

       

    }

    public async Task Update(ShippingInstruction si, string userId)
    {
        string sqlHeader = "sp_Wms_Shipping_Instruction_Update";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            si.ShippingInstructionNo,
            si.ShippingInstructionDate,
            si.PONumber,
            si.Supplier,
            Details = DataTableHelper.ToDataTable(si.Details),
            UpdateBy = userId
        });
    }

    public async Task Remove(string shippinginstructionno)
    {
        string sqlHeader = "sp_Wms_Shipping_Instruction_Delete";
        await _dbExecutor.ExecuteAsync(sqlHeader, new { ShippingInstructionNo = shippinginstructionno });
    }

    public async Task<Dictionary<string, object>> Capture(string shippinginstructionno)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_Shipping_Instruction_Capture",
            param: new { ShippingInstructionNo = shippinginstructionno },
            async multi =>
            {
                var header = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var details = (await multi.ReadAsync<dynamic>()).ToList();

                return (header, details);
            }
        );

        return new Dictionary<string, object>
        {
            { "Shipping InstructionNo Header", result.header },
            { "Shipping InstructionNo Detail", result.details }
        };
    }

    public async Task<List<ShippingInstructionReportDto>> GetListReport(string sino)
    {
        string sp = "sp_Wms_ShippingInstruction_Report";

        return (await _dbExecutor.QueryListAsync<ShippingInstructionReportDto>(sp, new
        {
            SINo = sino
        })).ToList();
    }


}
