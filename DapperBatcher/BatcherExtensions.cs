using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dapper;

namespace EdShel.DapperBatcher;

public static class BatcherExtensions
{
    private static ConditionalWeakTable<IDbConnection, ConnectionContext> batchContexts = new();

    public static IBatchedValue<IEnumerable<T>> QueryBatched<T>(this IDbConnection connection, string sql, object? param = null, CommandType? commandType = null)
    {
        var connectionProxy = new ProxyDbConnection(connection);
        var result = connectionProxy.Query(sql, param, commandType: commandType);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        var context = batchContexts.GetValue(
            connection,
            static connection => new ConnectionContext(connection)
        );
        var item = new BatchedValue<IEnumerable<T>>(
            context,
            proxyCommand,
            typeof(T)
        );
        context.BatchedCommands.Add(item);

        return item;
    }

    public static IBatchedValue<T?> QueryFirstOrDefaultBatched<T>(this IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        var connectionProxy = new ProxyDbConnection(connection);
        var result = connectionProxy.QueryFirstOrDefault(sql, param, transaction, commandTimeout, commandType);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        var context = batchContexts.GetValue(
            connection,
            static connection => new ConnectionContext(connection)
        );
        var item = new BatchedValue<T?>(
            context,
            proxyCommand,
            typeof(T)
        );
        context.BatchedCommands.Add(item);

        return item;
    }

    public static IBatchedValue<T> QueryFirstBatched<T>(this IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        var connectionProxy = new ProxyDbConnection(connection);
        var result = connectionProxy.QueryFirstOrDefault(sql, param, transaction, commandTimeout, commandType);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        var context = batchContexts.GetValue(
            connection,
            static connection => new ConnectionContext(connection)
        );
        var item = new BatchedValue<T>(
            context,
            proxyCommand,
            typeof(T)
        );
        context.BatchedCommands.Add(item);

        return item;
    }

    public static IBatchedValue<T> QuerySingleBatched<T>(this IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        var connectionProxy = new ProxyDbConnection(connection);
        var result = connectionProxy.QueryFirstOrDefault(sql, param, transaction, commandTimeout, commandType);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        var context = batchContexts.GetValue(
            connection,
            static connection => new ConnectionContext(connection)
        );
        var item = new BatchedValue<T>(
            context,
            proxyCommand,
            typeof(T)
        );
        context.BatchedCommands.Add(item);

        return item;
    }

    public static IBatchedValue<T?> QuerySingleOrDefaultBatched<T>(this IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        var connectionProxy = new ProxyDbConnection(connection);
        var result = connectionProxy.QueryFirstOrDefault(sql, param, transaction, commandTimeout, commandType);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        var context = batchContexts.GetValue(
            connection,
            static connection => new ConnectionContext(connection)
        );
        var item = new BatchedValue<T?>(
            context,
            proxyCommand,
            typeof(T)
        );
        context.BatchedCommands.Add(item);

        return item;
    }

}
