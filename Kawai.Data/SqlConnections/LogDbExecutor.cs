using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Kawai.Data.SqlConnections;

public class LogExecutor
{
    private readonly IConnectionFactory _connectionFactory;
    public LogExecutor(IConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// ambil list data dari raw SQL / stored procedure).
    /// </summary>
    public async Task<IEnumerable<T>> QueryListAsync<T>(string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure)
    {
        try
        {
            using var conn = _connectionFactory.GetDbLogConnection();
            conn.Open();
            return await conn.QueryAsync<T>(sql, param, commandType: commandType);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// ambil data satu-satunya dari raw SQL / stored procedure (kalau return nya > 1 data, maka dianggap error).
    /// </summary>
    public async Task<T?> QuerySingleOrDefaultAsync<T>(string sqlOrSp, object? param = null, CommandType commandType = CommandType.StoredProcedure)
    {
        try
        {
            using var conn = _connectionFactory.GetDbLogConnection();
            conn.Open();
            return await conn.QuerySingleOrDefaultAsync<T>(sqlOrSp, param, commandType: commandType);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// ambil data pertama dari raw SQL / stored procedure.
    /// </summary>
    public async Task<T?> QueryFirstOrDefaultAsync<T>(string sqlOrSp, object? param = null, CommandType commandType = CommandType.StoredProcedure)
    {
        try
        {
            using var conn = _connectionFactory.GetDbLogConnection();
            conn.Open();
            return await conn.QueryFirstOrDefaultAsync<T>(sqlOrSp, param, commandType: commandType);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// eksekusi command (insert, update, delete, dsb) => transaction (optional).
    /// </summary>
    public async Task<bool> ExecuteAsync(string sql, object? param = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.StoredProcedure)
    {
        try
        {
            using var conn = _connectionFactory.GetDbLogConnection();
            conn.Open();
            await conn.ExecuteAsync(sql, param, transaction, commandType: commandType);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    public bool Execute(string sql, object? param = null, IDbTransaction? transaction = null, CommandType commandType = CommandType.StoredProcedure)
    {
        try
        {
            using var conn = _connectionFactory.GetDbLogConnection();
            conn.Open();
            conn.Execute(sql, param, transaction, commandType: commandType);
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// eksekusi beberapa command => transaction (required).
    /// </summary>
    public async Task<bool> ExecuteTransactionAsync(IEnumerable<(string SqlOrSp, object? Param, CommandType CmdType)> commands)
    {
        using var connection = _connectionFactory.GetDbLogConnection();
        using var transaction = connection.BeginTransaction();

        try
        {
            foreach (var cmd in commands)
            {
                await connection.ExecuteAsync(cmd.SqlOrSp, cmd.Param, transaction, commandType: cmd.CmdType);
            }
            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
