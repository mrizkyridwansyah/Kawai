using Kawai.Domain.DTOs;
using Kawai.Domain.Models;

namespace Kawai.Domain.Interfaces;

public interface IReceiptUnscheduleRepository
{
    Task Create(ReceiptUnschedule receipt, string userId);
    Task Update(ReceiptUnschedule receipt, string userId);
}
