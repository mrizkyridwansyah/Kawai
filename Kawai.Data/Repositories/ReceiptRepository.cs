using Dapper;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;
using System.Data;


namespace Kawai.Data.Repositories;

public class ReceiptRepository : IReceiptRepository
{

    private readonly DbExecutor _dbExecutor;

    public ReceiptRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task Import( ReceiptHeaderImport header, DataTable dtDetail, string userId, string factoryCode)
    {
        string sql = @"sp_Wms_Receipt_Import";
        await _dbExecutor.ExecuteNonTransactionAsync(sql, new
        {
            header.SupplierCode,
            header.DNNumber,
            header.ReceiptDate,
            header.BCType,
            header.BCNumber,
            header.BCDate,
            DataImport = dtDetail,
            UserId = userId,
            FactoryCode = factoryCode
        });
        
        
    }

    public async Task<ReceiptImport> ValidateImport(
    ReceiptHeaderImport header,
    DataTable datas,
    string userId)
    {
        return await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_Receipt_ValidateImport",
            param: new
            {
                SupplierCode = header.SupplierCode,
                DNNumber = header.DNNumber,
                ReceiptDate = header.ReceiptDate,
                BCType = header.BCType,
                BCNumber = header.BCNumber,
                BCDate = header.BCDate,
                DataImport = datas,
                UserId = userId
            },
            async multi =>
            {
                var headerResult =
                    (await multi.ReadAsync<ReceiptHeaderImport>())
                    .FirstOrDefault();

                var detailResult =
                    (await multi.ReadAsync<ReceiptDetailImport>())
                    .ToList();

                return new ReceiptImport
                {
                    Header = headerResult,
                    Details = detailResult
                };
            }
        );
    }


    public async Task<List<ReceiptDto>> GetList(RequestParameter param)
    {
        string sp = "sp_Wms_Receipt_List";
        return (await _dbExecutor.QueryListAsync<ReceiptDto>(sp, param.ToQueryObject())).ToList();
    }
    public async Task<List<PODetailDto>> GetListPODetail(RequestParameter param)
    {
        var paramFactory = param.GetParam("FactoryCode");
        var paramReceiptId = param.GetParam("ReceiptId");
        var paramPONumber = param.GetParam("PONumber");
        var paramSupplier = param.GetParam("SupplierCode");
        var paramDateFrom = param.GetParam("DateFrom");
        var paramDateUntil = param.GetParam("DateUntil");

        string sp = "sp_Wms_Receipt_ListPODetail";
        return (await _dbExecutor.QueryListAsync<PODetailDto>(sp, new
        {
            FactoryCode = paramFactory,
            ReceiptId = paramReceiptId,
            PONumber = paramPONumber,
            SupplierCode = paramSupplier,
            DateFrom = paramDateFrom,
            DateUntil = paramDateUntil,
        })).ToList();
    }

    public async Task<List<ClaimDetailDto>> GetListClaimDetail(RequestParameter param)
    {
        var paramFactory = param.GetParam("FactoryCode");
        var paramReceiptId = param.GetParam("ReceiptId");
        var paramClaimNumber = param.GetParam("PONumber");
        var paramSupplier = param.GetParam("SupplierCode");
        var paramDateFrom = param.GetParam("DateFrom");
        var paramDateUntil = param.GetParam("DateUntil");

        string sp = "sp_Wms_Receipt_ListClaimDetail";
        return (await _dbExecutor.QueryListAsync<ClaimDetailDto>(sp, new
        {
            FactoryCode = paramFactory,
            ReceiptId = paramReceiptId,
            ClaimNumber = paramClaimNumber,
            SupplierCode = paramSupplier,
            DateFrom = paramDateFrom,
            DateUntil = paramDateUntil,
        })).ToList();
    }



    public async Task<List<LabelBarcodeDetailDto>> GetListBarcodeDetail(long id)
    {


        string sp = "sp_Wms_PartReceiptDetailBarcodeLabel";
        return (await _dbExecutor.QueryListAsync<LabelBarcodeDetailDto>(sp, new
        {
            ReceiptNo = id,

        })).ToList();
    }

    public async Task<ReceiptDto> GetDataHeader(long id)
    {
        string sp = "sp_Wms_Receipt_DataHeader";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ReceiptDto>(sp, new { ReceiptId = id });
    }
    public async Task<List<ReceiptDetailDto>> GetListDetail(long receiptId)
    {
        string sp = "sp_Wms_Receipt_ListDetail";
        return (await _dbExecutor.QueryListAsync<ReceiptDetailDto>(sp, new { ReceiptId = receiptId })).ToList();
    }

    public async Task<List<ReceiptDetailBarcodeDto>> GetListDetailBarcode(long receiptId)
    {
        string sp = "sp_Wms_Receipt_ListDetailBarcode";
        return (await _dbExecutor.QueryListAsync<ReceiptDetailBarcodeDto>(sp, new { ReceiptId = receiptId })).ToList();
    }

    public async Task<ReceiptDetailBarcodeDto> GetDataBarcode(string barcodeNo, string userId)
    {
        string sp = "sp_Wms_Receipt_DataBarcode";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ReceiptDetailBarcodeDto>(sp, new { BarcodeNo = barcodeNo, UserId = userId });
    }

    public async Task<List<ReceiptDto>> DDLSearch(string keyword, string factory, string supplier, DateTime? periodFrom, DateTime? periodUntil, string status, string sourceMenu, string userId)
    {
        string sp = "sp_Wms_Receipt_DDL";

        return (await _dbExecutor.QueryListAsync<ReceiptDto>(sp, new
        {
            Keyword = keyword ?? "",
            Status = status ?? "",
            SourceMenu = sourceMenu ?? "",
            FactoryCode = String.IsNullOrEmpty(factory) ? "ALL" : factory,
            SupplierCode = String.IsNullOrEmpty(supplier) ? "ALL" : supplier,
            PeriodFrom = periodFrom,
            PeriodUntil = periodUntil,
            UserId = userId
        })).ToList();
    }

    public async Task<List<ReceiptDto>> DDLSearchReceipt(string keyword, string userId)
    {
        string sp = "sp_Wms_Receipt_DDLSearchReceipt";
        return (await _dbExecutor.QueryListAsync<ReceiptDto>(sp, new { Keyword = keyword ?? "", UserId = userId })).ToList();
    }

    public async Task<List<ReceiptDto>> DNDDLSearch(string keyword, string factory, string supplier, DateTime? periodFrom, DateTime? periodUntil, string status, string userId)
    {
        string sp = "sp_Wms_Receipt_DDLDN";

        return (await _dbExecutor.QueryListAsync<ReceiptDto>(sp, new
        {
            Keyword = keyword ?? "",
            Status = status ?? "",
            FactoryCode = String.IsNullOrEmpty(factory) ? "ALL" : factory,
            SupplierCode = String.IsNullOrEmpty(supplier) ? "ALL" : supplier,
            PeriodFrom = periodFrom,
            PeriodUntil = periodUntil,
            UserId = userId
        })).ToList();
    }

    public async Task<List<PODto>> PODDLSearch(string keyword, string factory, string supplier, string typeDate, DateTime? periodFrom, DateTime? periodUntil, bool showOptionAll, string userId, long? receiptId)
    {
        string sp = "sp_Wms_Receipt_DDLPO";
        var today = DateTime.Today;
        var awalBulan = new DateTime(today.Year, today.Month, 1);

        return (await _dbExecutor.QueryListAsync<PODto>(sp, new
        {
            Keyword = keyword ?? "",
            FactoryCode = String.IsNullOrEmpty(factory) ? "ALL" : factory,
            SupplierCode = String.IsNullOrEmpty(supplier) ? "ALL" : supplier,
            TypeDate = typeDate,
            PeriodFrom = periodFrom.HasValue ? periodFrom.Value : awalBulan,
            PeriodUntil = periodUntil.HasValue ? periodUntil.Value : DateTime.Today,
            ReceiptId = receiptId,
            ShowOptionAll = showOptionAll,
            UserId = userId
        })).ToList();
    }

    public async Task Create(Receipt receipt, string userId)
    {
        receipt.ReceiptNo = await _dbExecutor.QuerySingleOrDefaultAsync<string>("sp_Wms_Receipt_GenerateCode", new { receipt.FactoryCode, receipt.ReceiptDate });
        string sqlHeader = "sp_Wms_Receipt_Create";
        long newId = await _dbExecutor.QuerySingleOrDefaultAsync<long>(sqlHeader, new
        {
            receipt.ReceiptNo,
            receipt.ReceiptDate,
            receipt.DNNumber,
            receipt.FactoryCode,
            receipt.SupplierCode,
            receipt.DNDate,
            receipt.BCNumber,
            receipt.BCType,
            receipt.BCDate,
            receipt.VehicleNo,
            receipt.Transport,
            receipt.Remarks,
            Details = DataTableHelper.ToDataTable(receipt.Details),
            RegisterBy = userId
        });
        receipt.Id = newId;
    }

    public async Task<ReceiptConfirmationCheckIsDetailsUpdateDto> CheckIsDetailsUpdate(Receipt receipt)
    {
        string sp = "sp_Wms_Receipt_CheckIsDetailsUpdate";
        return await _dbExecutor.QueryFirstOrDefaultAsync<ReceiptConfirmationCheckIsDetailsUpdateDto>(sp, new
        {
            receipt.Id,
            receipt.SupplierCode,
            Details = DataTableHelper.ToDataTable(receipt.Details)
        });
    }

    public async Task Update(Receipt receipt, string userId)
    {
        string sqlHeader = "sp_Wms_Receipt_Update";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            receipt.Id,
            receipt.ReceiptDate,
            receipt.DNNumber,
            receipt.FactoryCode,
            receipt.SupplierCode,
            receipt.DNDate,
            receipt.BCNumber,
            receipt.BCType,
            receipt.BCDate,
            receipt.VehicleNo,
            receipt.Transport,
            receipt.Remarks,
            receipt.RegisterNo,
            Details = DataTableHelper.ToDataTable(receipt.Details),
            UpdateBy = userId
        });
    }


    public async Task CreateClaim(Receipt receipt, string userId)
    {
        receipt.ReceiptNo = await _dbExecutor.QuerySingleOrDefaultAsync<string>("sp_Wms_Receipt_GenerateCode", new { receipt.FactoryCode, ReceiptDate = DateTime.Today });
        string sqlHeader = "sp_Wms_ReceiptClaim_Create";
        long newId = await _dbExecutor.QuerySingleOrDefaultAsync<long>(sqlHeader, new
        {
            receipt.ReceiptNo,
            receipt.DNNumber,
            receipt.FactoryCode,
            receipt.SupplierCode,
            receipt.DNDate,
            receipt.BCNumber,
            receipt.BCType,
            receipt.BCDate,
            receipt.VehicleNo,
            receipt.Transport,
            receipt.Remarks,
            Details = DataTableHelper.ToDataTable(receipt.Details),
            RegisterBy = userId
        });
        receipt.Id = newId;
    }

    public async Task UpdateClaim(Receipt receipt, string userId)
    {
        string sqlHeader = "sp_Wms_ReceiptClaim_Update";
        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            receipt.Id,
            receipt.DNNumber,
            receipt.FactoryCode,
            receipt.SupplierCode,
            receipt.DNDate,
            receipt.BCNumber,
            receipt.BCType,
            receipt.BCDate,
            receipt.VehicleNo,
            receipt.Transport,
            receipt.Remarks,
            receipt.RegisterNo,
            Details = DataTableHelper.ToDataTable(receipt.Details),
            UpdateBy = userId
        });
    }


    public async Task Remove(long id)
    {
        string sqlHeader = "sp_Wms_Receipt_Delete";
        await _dbExecutor.ExecuteAsync(sqlHeader, new { Id = id });
    }

    public async Task PrintLabel(long id, string userId, bool? mustBePrint)
    {
        string sqlHeader = "sp_Wms_Receipt_PrintLabel";
        await _dbExecutor.ExecuteNonTransactionAsync(sqlHeader, new
        {
            ReceiptId = id,
            MustPrint = mustBePrint ?? true,
            UserId = userId
        });
    }

    public async Task Verify(MobileReceipt payload, string userId)
    {
        string sqlHeader = "sp_Wms_Receipt_Verify";
        payload.RefNo = await _dbExecutor.QuerySingleOrDefaultAsync<string>("sp_Wms_Stock_GeneratePalletNo");

        await _dbExecutor.ExecuteAsync(sqlHeader, new
        {
            payload.RefNo,
            Details = DataTableHelper.ToDataTable(payload.Details),
            VerifiedBy = userId
        });
    }

    public async Task<List<ReceiptAndonDto>> GetListNSummary()
    {
        string sp = "sp_Wms_Andon_ReceiptGetList";
        return (await _dbExecutor.QueryListAsync<ReceiptAndonDto>(sp)).ToList();
    }

    public async Task<List<ReceiptAndonDto>> GetListNSummarybySupplier(string supplier)
    {
        string sp = "sp_Wms_Andon_ReceiptGetListBySupplier";
        return (await _dbExecutor.QueryListAsync<ReceiptAndonDto>(sp, new { Supplier = supplier ?? "ALL"})).ToList();
    }



    public async Task<Dictionary<string, object>> Capture(long id)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_Receipt_Capture",
            param: new { Id = id },
            async multi =>
            {
                var header = (await multi.ReadAsync<dynamic>()).FirstOrDefault();
                var details = (await multi.ReadAsync<dynamic>()).ToList();
                var detailBarcode = (await multi.ReadAsync<dynamic>()).ToList();
                foreach (var detail in details)
                {
                    detail.DetailBarcodes = detailBarcode
                    .Where(barcode =>
                        barcode.ReceiptId == detail.ReceiptId &&
                        barcode.ReceiptDetailId == detail.Id
                    )
                    .Select(p => new
                    {
                        p.BarcodeNo,
                        p.LotNo,
                        p.SublotNo,
                        p.Qty,
                        p.IsVerified,
                        p.VerifiedBy,
                        p.VerifiedDate
                    })
                    .ToList();
                }
                return (header, details);
            }
        );

        return new Dictionary<string, object>
        {
            { "Receipt Header", result.header },
            { "Receipt Detail", result.details }
        };
    }

    public async Task<Dictionary<string, object>> CaptureDataGrouping(string refNo)
    {
        var result = await _dbExecutor.QueryMultipleAsync(
            "sp_Wms_Receipt_CaptureBarcode",
            param: new { RefNo = refNo },
            async multi =>
            {
                var stocks = (await multi.ReadAsync<StockMasterDto>()).ToList();
                var stockDetail = (await multi.ReadAsync<StockDetailDto>()).ToList();
                foreach (var master in stocks)
                {
                    master.StockDetails = stockDetail
                    .Where(detail =>
                        detail.RefNo == master.RefNo &&
                        detail.WarehouseCode == master.WarehouseCode &&
                        detail.AreaCode == master.AreaCode &&
                        detail.ItemCode == master.ItemCode &&
                        detail.LotNo == master.LotNo
                    ).ToList();
                }
                return stocks;
            }
        );

        return new Dictionary<string, object>
        {
            { "Pallet No: ", refNo },
            { "Stock", result }
        };
    }
    public async Task<List<ReceiptInquiryDto>> Inquiry(RequestParameter param)
    {
        string sp = "sp_Wms_Receipt_Inquiry";
        return (await _dbExecutor.QueryListAsync<ReceiptInquiryDto>(sp, param.ToQueryObject())).ToList();
    }
    public async Task<List<ReceiptDetailBarcodeDto>> InquiryDetail(RequestParameter param)
    {
        string sp = "sp_Wms_Receipt_InquiryDetail";
        return (await _dbExecutor.QueryListAsync<ReceiptDetailBarcodeDto>(sp, param.ToQueryObject())).ToList();
    }

}
