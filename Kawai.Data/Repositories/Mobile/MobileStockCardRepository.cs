using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces.Mobile;

namespace Kawai.Data.Repositories.Mobile;

public class MobileStockCardRepository : IMobileStockCardRepository
{
    private readonly DbExecutor _dbExecutor;

    public MobileStockCardRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<StockCardDto>> GetListStockCard(
        string? itemCode)
    {
        var result = await _dbExecutor.QueryListAsync<StockCardDto>(
            "sp_Wms_Mobile_GetListStockCard",
            new
            {
                ItemCode = itemCode
            });

        return result.ToList();
    }

    public async Task<List<ItemDto>> GetDDLItemCode(string keyword)
    {
        string sp = "sp_Wms_Mobile_StockCard_GetItemCode";
        return (await _dbExecutor.QueryListAsync<ItemDto>(sp, new { Keyword = keyword ?? "" })).ToList();
    }
}