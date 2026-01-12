using Kawai.Domain.DTOs;
using Kawai.Domain.Models;
using Kawai.Domain.Models.Mobile;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IInventoryClosingRepository
{
    Task<InventoryClosingDto> GetCurrent();
    Task ProcessClosing(int ivtYear, int ivtMonth, string userId);
}
