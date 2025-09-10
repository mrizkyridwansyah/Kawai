using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Data.Repositories;

public class TradeRepository : ITradeRepository
{
    private readonly DbExecutor _dbExecutor;

    public TradeRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<TradeDto>> GetAll(RequestParameter param)
    {
        string sp = "sp_Wms_Trade_List";
        return (await _dbExecutor.QueryListAsync<TradeDto>(sp, param.ToQueryObject())).ToList();
    }

    public async Task<TradeDto> GetData(string trade_Code)
    {
        string sp = "sp_Wms_Trade_GetDetail";
        return await _dbExecutor.QueryFirstOrDefaultAsync<TradeDto>(sp, new { Trade_Code = trade_Code });
    }

    public async Task<List<TradeDto>> GetDDL(string keyword)
    {
        string sp = "sp_Wms_Trade_DDL";
        return (await _dbExecutor.QueryListAsync<TradeDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }

    

    public async Task Create(Trade trade, string userId)
    {
        string sql = @"sp_Wms_Trade_Create";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            trade.Trade_Code,
            trade.Trade_Cls,
            trade.Trade_Name,
            trade.Trade_Abbr,
            trade.Contact_Person,
            trade.Address1,
            trade.Address2,
            trade.City,
            trade.Country,
            trade.Country_Cls,
            trade.Epte_Cls,
            trade.Region_Cls,
            trade.Postal_Code,
            trade.Telephone,
            trade.Fax,
            trade.Closing_Day,
            trade.Pay_Day,
            trade.InvoicePay_Days,
            trade.Affiliate_Cls,
            trade.Insurance_Cls,
            trade.NPWP_No,
            trade.NPWP_Name,
            trade.NPWP_Address,
            trade.NPWP_City,
            trade.NPPKP_No,
            trade.Invoice_To,
            trade.PO_Cls,
            trade.Price_Condition,
            trade.POPayment_Day,
            trade.POPayment_Terms,
            trade.Transportation_Cls,
            trade.POCaseMark1,
            trade.POCaseMark2,
            trade.POCaseMark3,
            trade.POCaseMark4,
            trade.POCaseMark5,
            trade.POMarking1,
            trade.POMarking2,
            trade.POMarking3,
            trade.POMarking4,
            trade.POMarking5,
            trade.POMarking6,
            trade.Subcon_WH_Code,
            trade.NG_Cls,
            trade.SAP_Code,
            trade.Type_BC,
            trade.No_Izin,
            trade.CODE_KPPBC,
            trade.NoIzin_Date,
            trade.NITKU,
            RegisterBy = userId
 
        });
    }

    public async Task Update(Trade trade, string userId)
    {
        string sql = @"sp_Wms_Trade_Update";
        int i = await _dbExecutor.ExecuteAsync(sql, new
        {
            trade.Trade_Code,
            trade.Trade_Cls,
            trade.Trade_Name,
            trade.Trade_Abbr,
            trade.Contact_Person,
            trade.Address1,
            trade.Address2,
            trade.City,
            trade.Country,
            trade.Country_Cls,
            trade.Epte_Cls,
            trade.Region_Cls,
            trade.Postal_Code,
            trade.Telephone,
            trade.Fax,
            trade.Closing_Day,
            trade.Pay_Day,
            trade.InvoicePay_Days,
            trade.Affiliate_Cls,
            trade.Insurance_Cls,
            trade.NPWP_No,
            trade.NPWP_Name,
            trade.NPWP_Address,
            trade.NPWP_City,
            trade.NPPKP_No,
            trade.Invoice_To,
            trade.PO_Cls,
            trade.Price_Condition,
            trade.POPayment_Day,
            trade.POPayment_Terms,
            trade.Transportation_Cls,
            trade.POCaseMark1,
            trade.POCaseMark2,
            trade.POCaseMark3,
            trade.POCaseMark4,
            trade.POCaseMark5,
            trade.POMarking1,
            trade.POMarking2,
            trade.POMarking3,
            trade.POMarking4,
            trade.POMarking5,
            trade.POMarking6,
            trade.Subcon_WH_Code,
            trade.NG_Cls,
            trade.SAP_Code,
            trade.Type_BC,
            trade.No_Izin,
            trade.CODE_KPPBC,
            trade.NoIzin_Date,
            trade.NITKU,
            UpdateBy = userId
        });
    }

    public async Task Remove(string trade_Code, string userId)
    {
        string sql = "sp_Wms_Trade_Delete";
        int i = await _dbExecutor.ExecuteAsync(sql, new { Trade_Code = trade_Code });
    }

    public async Task<Dictionary<string, object>> Capture(string trade_Code)
    {
        string sp = "sp_Wms_Trade_Capture";
        var result = await _dbExecutor.QueryFirstOrDefaultAsync<dynamic>(sp, new { Trade_Code = trade_Code });

        if (result == null)
            return new Dictionary<string, object>();

        return ((IDictionary<string, object>)result).ToDictionary(k => k.Key, v => v.Value);
    }

   
}
