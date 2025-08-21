namespace Kawai.Domain.Interfaces.Mobile;

public interface IMobileRepository
{
    Task<string> GetLastVersion();
}
