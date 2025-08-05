using System.Data;

namespace Kawai.Data.SqlConnections;

public interface IConnectionFactory
{
    IDbConnection GetDbConnection();
    IDbConnection GetDbLogConnection();
}
