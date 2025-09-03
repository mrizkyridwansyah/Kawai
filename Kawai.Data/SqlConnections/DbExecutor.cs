using Dapper;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Transactions;

namespace Kawai.Data.SqlConnections;

public class DbExecutor
{
    private readonly IConnectionFactory _connectionFactory;
    public DbExecutor(IConnectionFactory connectionFactory)
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
            using var conn = _connectionFactory.GetDbConnection();
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
            using var conn = _connectionFactory.GetDbConnection();
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
            using var conn = _connectionFactory.GetDbConnection();
            conn.Open();
            return await conn.QueryFirstOrDefaultAsync<T>(sqlOrSp, param, commandType: commandType);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }

    /// <summary>
    /// Eksekusi stored procedure yang mengembalikan 2 result sets.
    /// </summary>
    public async Task<TResult> QueryMultipleAsync<TResult>(
    string sqlOrSp,
        object? param,
        Func<SqlMapper.GridReader, Task<TResult>> readerFunc,
        CommandType commandType = CommandType.StoredProcedure)
    {
        try
        {
            using var conn = _connectionFactory.GetDbConnection();
            conn.Open();

            using var multi = await conn.QueryMultipleAsync(sqlOrSp, param, commandType: commandType);
            return await readerFunc(multi);
        }
        catch (Exception ex)
        {
            throw new Exception($"QueryMultipleAsync failed: {ex.Message}", ex);
        }
    }


    /// <summary>
    /// eksekusi command (insert, update, delete, dsb) => transaction (optional).
    /// </summary>
    public async Task<int> ExecuteAsync(string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure)
    {
        using var conn = _connectionFactory.GetDbConnection();
        conn.Open();

        using var transaction = conn.BeginTransaction();

        try
        {
            var result = await conn.ExecuteAsync(sql, param, transaction, commandType: commandType);
            transaction.Commit();
            return result;
        }
        catch (SqlException)
        {
            transaction.Rollback();
            throw;
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }

    }

    public async Task<int> ExecuteNonTransactionAsync(string sql, object? param = null, CommandType commandType = CommandType.StoredProcedure)
    {
        try
        {
            using var conn = _connectionFactory.GetDbConnection();
            conn.Open();
            return await conn.ExecuteAsync(sql, param, commandType: commandType);
        }
        catch (SqlException)
        {
            throw;
        }
        catch (Exception)
        {
            throw;
        }

    }

    /// <summary>
    /// eksekusi beberapa command => transaction (required).
    /// </summary>
    public async Task<int> ExecuteTransactionAsync(IEnumerable<(string SqlOrSp, object? Param, CommandType CmdType)> commands)
    {
        using var connection = _connectionFactory.GetDbConnection();
        connection.Open();
        using var transaction = connection.BeginTransaction();

        try
        {
            foreach (var cmd in commands)
            {
                int i = await connection.ExecuteAsync(cmd.SqlOrSp, cmd.Param, transaction, commandType: cmd.CmdType);
            }
            transaction.Commit();
            return 1;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
