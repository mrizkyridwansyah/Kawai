using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IInventoryReportByItemRepository
{
    Task<List<InventoryReportByItemDto>> GetAll(RequestParameter param);
    Task<List<InventoryListReceiptSupplyReportDto>> GetListReceiptSupply(RequestParameter param);
}