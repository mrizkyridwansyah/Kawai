using Kawai.Data.SqlConnections;
using Kawai.Domain.DTOs;
using Kawai.Domain.Interfaces;

namespace Kawai.Data.Repositories;

public class StockMutationRepository: IStockMutationRepository
{
    private readonly DbExecutor _dbExecutor;

    public StockMutationRepository(DbExecutor dbExecutor)
    {
        _dbExecutor = dbExecutor;
    }

    public async Task<List<StockMutationDto>> GetPendingStock()
    {
        return (await _dbExecutor.QueryListAsync<StockMutationDto>("sp_Wms_StockCalculate_GetPendingStock")).ToList();
    }

    public async Task Receipt(string id)
    {
        await _dbExecutor.ExecuteNonTransactionAsync("sp_Wms_StockCalculate_ReceiptBatch", new { SourceRef =id });
    }

    public async Task Production(string id)
    {
        await _dbExecutor.ExecuteNonTransactionAsync("sp_Wms_StockCalculate_ProductionBatch", new { SourceRef =id });
    }

    public async Task Adjustment(string id)
    {
        await _dbExecutor.ExecuteNonTransactionAsync("sp_Wms_StockCalculate_AdjustmentBatch", new { SourceRef =id });
    }

    public async Task Transfer(string id)
    {
        await _dbExecutor.ExecuteNonTransactionAsync("sp_Wms_StockCalculate_TransferBatch", new { SourceRef =id });
    }

    public async Task PrepareConsume(string id)
    {
        await _dbExecutor.ExecuteNonTransactionAsync("sp_Wms_StockCalculate_PrepareConsumeBatch", new { SourceRef =id });
    }

    public async Task UsedConsume(string id)
    {
        await _dbExecutor.ExecuteNonTransactionAsync("sp_Wms_StockCalculate_UsedConsumeBatch", new { SourceRef =id });
    }

    public async Task Split(string id)
    {
        await _dbExecutor.ExecuteNonTransactionAsync("sp_Wms_StockCalculate_SplitBatch", new { SourceRef =id });
    }
}
