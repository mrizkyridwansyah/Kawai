using Kawai.Domain.DTOs;
using Kawai.Domain.Shared;

namespace Kawai.Domain.Interfaces;

public interface IDeliveryNoteRepository
{
    Task<List<DeliveryNoteDto>> GetList(RequestParameter parameter);
    Task<DeliveryNoteDto> GetDetail(string dnNumber);
    Task<List<DeliveryNoteDetailDto>> GetListDetail(string dnNumber);
}
