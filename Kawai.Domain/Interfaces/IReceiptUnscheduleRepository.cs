using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IReceiptUnscheduleRepository
{
    Task<List<ItemPackingSupplierDto>> GetListItem(RequestParameter param);
    Task Create(ReceiptUnschedule receipt, string userId);
    Task<ReceiptConfirmationCheckIsDetailsUpdateDto> CheckIsDetailsUpdate(ReceiptUnschedule receipt);
    Task Update(ReceiptUnschedule receipt, string userId);
}
