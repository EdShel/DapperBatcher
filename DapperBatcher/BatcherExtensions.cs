using System.Data;

namespace EdShel.DapperBatcher;

public static class BatcherExtensions
{
    private static readonly DapperCommandsBatcher globalBatcher = new();

    public static IBatchedValue<IEnumerable<T>> QueryBatched<T>(this IDbConnection connection, string sql, object? param = null, CommandType? commandType = null)
    {
        return globalBatcher.QueryBatched<T>(connection, sql, param, commandType);
    }

    public static IBatchedValue<T> QueryFirstBatched<T>(this IDbConnection connection, string sql, object? param = null, CommandType? commandType = null)
    {
        return globalBatcher.QueryFirstBatched<T>(connection, sql, param, commandType);
    }

    public static IBatchedValue<T?> QueryFirstOrDefaultBatched<T>(this IDbConnection connection, string sql, object? param = null, CommandType? commandType = null)
    {
        return globalBatcher.QueryFirstOrDefaultBatched<T>(connection, sql, param, commandType);
    }

    public static IBatchedValue<T> QuerySingleBatched<T>(this IDbConnection connection, string sql, object? param = null, CommandType? commandType = null)
    {
        return globalBatcher.QuerySingleBatched<T>(connection, sql, param, commandType);
    }

    public static IBatchedValue<T?> QuerySingleOrDefaultBatched<T>(this IDbConnection connection, string sql, object? param = null, CommandType? commandType = null)
    {
        return globalBatcher.QuerySingleOrDefaultBatched<T>(connection, sql, param, commandType);
    }

    public static IBatchedValue<int> ExecuteBatched(this IDbConnection connection, string sql, object? param = null, CommandType? commandType = null)
    {
        return globalBatcher.ExecuteBatched(connection, sql, param, commandType);
    }
}
