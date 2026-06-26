using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;
using System.Data;

namespace Kawai.Domain.Interfaces;

public interface IReceiptRepository
{
    #region WEB
    Task<List<ReceiptDto>> GetList(RequestParameter parameter);
    Task<List<PODetailDto>> GetListPODetail(RequestParameter parameter);
    Task<List<ClaimDetailDto>> GetListClaimDetail(RequestParameter parameter);
    Task<List<LabelBarcodeDetailDto>> GetListBarcodeDetail(long id);
    Task<ReceiptDto> GetDataHeader(long id);
    Task Create(Receipt receipt, string userId);

    Task<ReceiptConfirmationCheckIsDetailsUpdateDto> CheckIsDetailsUpdate(Receipt receipt);
    Task Update(Receipt receipt, string userId);

    Task<List<ReceiptBreakdownDto>> GetListBreakdownReceipt(long receiptId, long receiptDetailId);
    Task SaveBreakdownReceipt(ReceiptBreakdown payload, string userId);

    Task CreateClaim(Receipt receipt, string userId);
    Task UpdateClaim(Receipt receipt, string userId);
    Task Remove(long id);
    Task PrintLabel(long id, string userId, bool? mustBePrint);
    Task<List<ReceiptInquiryDto>> Inquiry(RequestParameter parameter);
    Task<List<ReceiptDetailBarcodeDto>> InquiryDetail(RequestParameter parameter);
    #endregion

    #region MOBILE
    Task<ReceiptDetailBarcodeDto> GetDataBarcode(string barcodeNo, string userId);
    Task Verify(MobileReceipt payload, string userId);
    #endregion

    #region COMMON
    Task<List<ReceiptDetailDto>> GetListDetail(long receiptId);
    Task<List<ReceiptDetailBarcodeDto>> GetListDetailBarcode(long receiptId);
    #endregion

    #region ANDON
    Task<List<ReceiptAndonDto>> GetListNSummary();
    Task<List<ReceiptAndonDto>> GetListNSummarybySupplier(string supplier);
    #endregion

    Task<List<ReceiptDto>> DDLSearch(string keyword, string factory, string supplier, DateTime? periodFrom, DateTime? periodUntil, string status, string sourceMenu, string userId);
    Task<List<ReceiptDto>> DDLSearchReceipt(string keyword, string userId);

    Task<List<ReceiptDto>> DNDDLSearch(string keyword, string factory, string supplier, DateTime? periodFrom, DateTime? periodUntil, string status, string userId);
    Task<List<PODto>> PODDLSearch(string keyword, string factory, string supplier, string typeDate, DateTime? periodFrom, DateTime? periodUntil, bool showOptionAll, string userId, long? receiptId);


    #region Import

    Task<ReceiptImport> ValidateImport(
     ReceiptHeaderImport header,
     DataTable datas,
     string userId);

    Task Import(
        ReceiptHeaderImport header,
        DataTable datas,
        string userId, string factoryCode);

    #endregion


    Task<Dictionary<string, object>> Capture(long id);
    Task<Dictionary<string, object>> CaptureDataGrouping(string refNo);
    Task<Dictionary<string, object>> CaptureBreakdown(long receiptDetailId);
}
