using Kawai.Api.Models;
using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;
using System.Data;
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

    public async Task<List<ListDeliveryPlaceDto>> GetDeliveryList(string trade_code)
    {
        string sp = "sp_Wms_TradeDeliveryPlace_Detail";
        return (await _dbExecutor.QueryListAsync<ListDeliveryPlaceDto>(sp, new { Trade_Code = trade_code })).ToList();
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

    public async Task<List<TradeDto>> GetDDLCustomer()
    {
        string sp = "sp_Wms_Trade_Customer";
        var data = (await _dbExecutor.QueryListAsync<TradeDto>(sp)).ToList();

        foreach (var item in data)
        {
            if (string.Equals(item.Trade_Code, "All", StringComparison.OrdinalIgnoreCase))
            {
                item.Trade_Code = "ALL";
                item.Trade_Name = "ALL";
            }

            item.DDLDescription = $"{item.Trade_Code} - {item.Trade_Name}";
        }

        return data;
    }


    public async Task SaveTradeDelivery(TradeSaveDelivery tradedeliverylist,string userId)
    {

        var commands = new List<(string, object?, CommandType)>();  // 1️⃣ SAVE / UPDATE TRADE (HEADER)
        commands.Add(("sp_Wms_Trade_InsUpd", new
        {
            tradedeliverylist.Trade_Code,
            tradedeliverylist.Trade_Cls,
            tradedeliverylist.Trade_Name,
            tradedeliverylist.Trade_Abbr,
            tradedeliverylist.Contact_Person,
            tradedeliverylist.Address1,
            tradedeliverylist.Address2,
            tradedeliverylist.City,
            tradedeliverylist.Country,
            tradedeliverylist.Country_Cls,
            tradedeliverylist.Epte_Cls,
            tradedeliverylist.Region_Cls,
            tradedeliverylist.Postal_Code,
            tradedeliverylist.Telephone,
            tradedeliverylist.Fax,
            tradedeliverylist.Closing_Day,
            tradedeliverylist.Pay_Day,
            tradedeliverylist.InvoicePay_Days,
            tradedeliverylist.Affiliate_Cls,
            tradedeliverylist.Insurance_Cls,
            tradedeliverylist.NPWP_No,
            tradedeliverylist.NPWP_Name,
            tradedeliverylist.NPWP_Address,
            tradedeliverylist.NPWP_City,
            tradedeliverylist.NPPKP_No,
            tradedeliverylist.Invoice_To,
            tradedeliverylist.PO_Cls,
            tradedeliverylist.Price_Condition,
            tradedeliverylist.POPayment_Day,
            tradedeliverylist.POPayment_Terms,
            tradedeliverylist.Transportation_Cls,
            tradedeliverylist.POCaseMark1,
            tradedeliverylist.POCaseMark2,
            tradedeliverylist.POCaseMark3,
            tradedeliverylist.POCaseMark4,
            tradedeliverylist.POCaseMark5,
            tradedeliverylist.POMarking1,
            tradedeliverylist.POMarking2,
            tradedeliverylist.POMarking3,
            tradedeliverylist.POMarking4,
            tradedeliverylist.POMarking5,
            tradedeliverylist.POMarking6,
            tradedeliverylist.Subcon_WH_Code,
            tradedeliverylist.NG_Cls,
            tradedeliverylist.SAP_Code,
            tradedeliverylist.Type_BC,
            tradedeliverylist.No_Izin,
            tradedeliverylist.CODE_KPPBC,
            tradedeliverylist.NoIzin_Date,
            tradedeliverylist.NITKU,
            RegisterBy = userId
        }, CommandType.StoredProcedure));

        // 2️⃣ DELETE DELIVERY LAMA
        commands.Add(("sp_WMS_TradeDelivery_Delete", new
        {
            Trade_Code = tradedeliverylist.Trade_Code
        }, CommandType.StoredProcedure));

        // 3️⃣ INSERT / UPDATE DELIVERY BARU
        if (tradedeliverylist.DeliveryList != null)
        {
            foreach (var wsSet in tradedeliverylist.DeliveryList)
            {
                commands.Add(("sp_WMS_TradeDelivery_Upd", new
                {
                    Trade_Code = tradedeliverylist.Trade_Code,
                    wsSet.Location_Code,
                    wsSet.Location_Name,
                    UserID = userId
                }, CommandType.StoredProcedure));
            }
        }

        // 4️⃣ EXECUTE SEMUA DALAM 1 TRANSAKSI
        await _dbExecutor.ExecuteMultiCommandWithTransactionAsync(commands);
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
