using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;

namespace Kawai.Data.SqlConnections;

public class ConnectionFactory : IConnectionFactory
{
    private readonly IConfiguration _configuration;

    public ConnectionFactory(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public IDbConnection GetDbConnection()
    {
        var connStr = _configuration.GetConnectionString("Db");
        return new SqlConnection(connStr);
    }

    public IDbConnection GetDbLogConnection()
    {
        var connStr = _configuration.GetConnectionString("DbLog");
        return new SqlConnection(connStr);
    }
}