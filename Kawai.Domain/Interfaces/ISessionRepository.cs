using Kawai.Api.Models;

namespace Kawai.Domain.Interfaces;

public interface ISessionRepository
{
    void AddSession(Session session);
}
