using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dapper;

namespace EdShel.DapperBatcher;

public static class BatcherExtensions
{
    private static ConditionalWeakTable<IDbConnection, ConnectionContext> batchContexts = new();

    private static ConnectionContext GetConnectionContext(IDbConnection connection)
    {
        return batchContexts.GetValue(
            connection,
            static connection => new ConnectionContext(connection)
        );
    }

    public static IBatchedValue<IEnumerable<T>> QueryBatched<T>(this IDbConnection connection, string sql, object? param = null, CommandType? commandType = null)
    {
        var connectionProxy = new ProxyDbConnection(connection);
        _ = connectionProxy.Query(sql, param, commandType: commandType);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        ConnectionContext context = GetConnectionContext(connection);
        var item = new BatchedMultiValue<T>(context);
        context.AddToBatch(item, proxyCommand);

        return item;
    }

    public static IBatchedValue<T?> QueryFirstOrDefaultBatched<T>(this IDbConnection connection, string sql, object? param = null, CommandType? commandType = null)
    {
        var connectionProxy = new ProxyDbConnection(connection);
        _ = connectionProxy.QueryFirstOrDefault(sql, param, commandType: commandType);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        ConnectionContext context = GetConnectionContext(connection);
        var item = new BatchedSingleValue<T?>(context, BatchedCommandKind.FirstOrDefault);
        context.AddToBatch(item, proxyCommand);

        return item;
    }

    public static IBatchedValue<T> QueryFirstBatched<T>(this IDbConnection connection, string sql, object? param = null, CommandType? commandType = null)
    {
        var connectionProxy = new ProxyDbConnection(connection);
        _ = connectionProxy.QueryFirstOrDefault(sql, param, commandType: commandType);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        ConnectionContext context = GetConnectionContext(connection);
        var item = new BatchedSingleValue<T>(context, BatchedCommandKind.First);
        context.AddToBatch(item, proxyCommand);

        return item;
    }

    public static IBatchedValue<T?> QuerySingleOrDefaultBatched<T>(this IDbConnection connection, string sql, object? param = null, CommandType? commandType = null)
    {
        var connectionProxy = new ProxyDbConnection(connection);
        _ = connectionProxy.QueryFirstOrDefault(sql, param, commandType: commandType);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        ConnectionContext context = GetConnectionContext(connection);
        var item = new BatchedSingleValue<T?>(context, BatchedCommandKind.SingleOrDefault);
        context.AddToBatch(item, proxyCommand);

        return item;
    }

    public static IBatchedValue<T> QuerySingleBatched<T>(this IDbConnection connection, string sql, object? param = null, CommandType? commandType = null)
    {
        var connectionProxy = new ProxyDbConnection(connection);
        _ = connectionProxy.QueryFirstOrDefault(sql, param, commandType: commandType);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        ConnectionContext context = GetConnectionContext(connection);
        var item = new BatchedSingleValue<T>(context, BatchedCommandKind.Single);
        context.AddToBatch(item, proxyCommand);

        return item;
    }

    public static IBatchedValue<int> ExecuteBatched(this IDbConnection connection, string sql, object? param = null, CommandType? commandType = null)
    {
        var connectionProxy = new ProxyDbConnection(connection);
        _ = connectionProxy.Execute(sql, param, commandType: commandType);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        ConnectionContext context = GetConnectionContext(connection);
        var item = new BatchedNonQueryValue(context);
        context.AddToBatch(item, proxyCommand);

        return item;
    }
}
