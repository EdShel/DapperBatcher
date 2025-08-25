using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Dapper;
using EdShel.DapperBatcher.Proxies;

namespace EdShel.DapperBatcher.Batching;

public class DapperCommandsBatcher
{
    internal ConditionalWeakTable<IDbConnection, ConnectionContext> Contexts = new();

    private ConnectionContext GetConnectionContext(IDbConnection connection)
    {
        return Contexts.GetValue(
            connection,
            static connection => new ConnectionContext(connection)
        );
    }

    public IBatchedValue<IEnumerable<T>> QueryBatched<T>(IDbConnection connection, string sql, object? param = null)
    {
        var connectionProxy = new ProxyDbConnection();
        _ = connectionProxy.Query(sql, param);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        ConnectionContext context = GetConnectionContext(connection);
        var item = new MultiValue<T>(context);
        context.AddToBatch(item, proxyCommand);

        return item;
    }

    public IBatchedValue<T?> QueryFirstOrDefaultBatched<T>(IDbConnection connection, string sql, object? param = null)
    {
        var connectionProxy = new ProxyDbConnection();
        _ = connectionProxy.Query(sql, param);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        ConnectionContext context = GetConnectionContext(connection);
        var item = new FirstOrSingleValue<T?>(context, FirstOrSingleKind.FirstOrDefault);
        context.AddToBatch(item, proxyCommand);

        return item;
    }

    public IBatchedValue<T> QueryFirstBatched<T>(IDbConnection connection, string sql, object? param = null)
    {
        var connectionProxy = new ProxyDbConnection();
        _ = connectionProxy.Query(sql, param);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        ConnectionContext context = GetConnectionContext(connection);
        var item = new FirstOrSingleValue<T>(context, FirstOrSingleKind.First);
        context.AddToBatch(item, proxyCommand);

        return item;
    }

    public IBatchedValue<T?> QuerySingleOrDefaultBatched<T>(IDbConnection connection, string sql, object? param = null)
    {
        var connectionProxy = new ProxyDbConnection();
        _ = connectionProxy.Query(sql, param);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        ConnectionContext context = GetConnectionContext(connection);
        var item = new FirstOrSingleValue<T?>(context, FirstOrSingleKind.SingleOrDefault);
        context.AddToBatch(item, proxyCommand);

        return item;
    }

    public IBatchedValue<T> QuerySingleBatched<T>(IDbConnection connection, string sql, object? param = null)
    {
        var connectionProxy = new ProxyDbConnection();
        _ = connectionProxy.Query(sql, param);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        ConnectionContext context = GetConnectionContext(connection);
        var item = new FirstOrSingleValue<T>(context, FirstOrSingleKind.Single);
        context.AddToBatch(item, proxyCommand);

        return item;
    }

    public IBatchedValue<int> ExecuteBatched(IDbConnection connection, string sql, object? param = null)
    {
        var connectionProxy = new ProxyDbConnection();
        _ = connectionProxy.Execute(sql, param);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        ConnectionContext context = GetConnectionContext(connection);
        var item = new NonQueryValue(context);
        context.AddToBatch(item, proxyCommand);

        return item;
    }
}
