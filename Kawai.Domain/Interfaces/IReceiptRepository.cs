using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IReceiptRepository
{
    #region WEB
    Task<List<ReceiptDto>> GetList(RequestParameter parameter);
    Task<List<PODetailDto>> GetListPODetail(RequestParameter parameter);
    Task<ReceiptDto> GetDataHeader(long id);
    Task Create(Receipt receipt, string userId);
    Task Update(Receipt receipt, string userId);
    Task Remove(long id);
    #endregion

    #region MOBILE
    Task<ReceiptDetailBarcodeDto> GetDataBarcode(long id, string barcodeNo);
    Task Verify(MobileReceipt payload, string userId);
    #endregion

    #region COMMON
    Task<List<ReceiptDetailDto>> GetListDetail(long receiptId);
    Task<List<ReceiptDetailBarcodeDto>> GetListDetailBarcode(long receiptId);
    #endregion

    #region ANDON
    Task<List<ReceiptAndonDto>> GetListNSummary();
    #endregion

    Task<List<ReceiptDto>> DDLSearch(string keyword, string supplier, DateTime? periodFrom, DateTime? periodUntil, string status, string sourceMenu);
    Task<List<ReceiptDto>> DDLSearchReceipt(string keyword);


    Task<Dictionary<string, object>> Capture(long id);
    Task<Dictionary<string, object>> CaptureDataBarcode(long id);
}
