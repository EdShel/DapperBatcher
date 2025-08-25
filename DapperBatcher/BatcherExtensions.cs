using System.Data;
using EdShel.DapperBatcher.Batching;

namespace EdShel.DapperBatcher;

public static class BatcherExtensions
{
    private static readonly DapperCommandsBatcher globalBatcher = new();

    /// <summary>
    /// Batches a DB command to return a sequence of objects.
    /// </summary>
    /// <typeparam name="T">Type of element in the sequence.</typeparam>
    /// <param name="connection">DB connection to use.</param>
    /// <param name="sql">SQL text of the command. May or may not end with semicolon.</param>
    /// <param name="param">Optional parameters for the command.</param>
    public static IBatchedValue<IEnumerable<T>> QueryBatched<T>(this IDbConnection connection, string sql, object? param = null)
    {
        return globalBatcher.QueryBatched<T>(connection, sql, param);
    }

    /// <summary>
    /// Batches a DB command to return the first object in the sequence. Throws an exception if the sequence is empty.
    /// </summary>
    /// <typeparam name="T">Type of element in the sequence.</typeparam>
    /// <param name="connection">DB connection to use.</param>
    /// <param name="sql">SQL text of the command. May or may not end with semicolon.</param>
    /// <param name="param">Optional parameters for the command.</param>
    public static IBatchedValue<T> QueryFirstBatched<T>(this IDbConnection connection, string sql, object? param = null)
    {
        return globalBatcher.QueryFirstBatched<T>(connection, sql, param);
    }

    /// <summary>
    /// Batches a DB command to return the first object in the sequence or the default value if the sequence is empty.
    /// </summary>
    /// <typeparam name="T">Type of element in the sequence.</typeparam>
    /// <param name="connection">DB connection to use.</param>
    /// <param name="sql">SQL text of the command. May or may not end with semicolon.</param>
    /// <param name="param">Optional parameters for the command.</param>
    public static IBatchedValue<T?> QueryFirstOrDefaultBatched<T>(this IDbConnection connection, string sql, object? param = null)
    {
        return globalBatcher.QueryFirstOrDefaultBatched<T>(connection, sql, param);
    }

    /// <summary>
    /// Batches a DB command to return the first object in the sequence. Throws an exception if the sequence is empty or has more than one element.
    /// </summary>
    /// <typeparam name="T">Type of element in the sequence.</typeparam>
    /// <param name="connection">DB connection to use.</param>
    /// <param name="sql">SQL text of the command. May or may not end with semicolon.</param>
    /// <param name="param">Optional parameters for the command.</param>
    public static IBatchedValue<T> QuerySingleBatched<T>(this IDbConnection connection, string sql, object? param = null)
    {
        return globalBatcher.QuerySingleBatched<T>(connection, sql, param);
    }

    /// <summary>
    /// Batches a DB command to return the first object in the sequence or the default value if the sequence is empty. Throws an exception if the sequence has more than one element.
    /// </summary>
    /// <typeparam name="T">Type of element in the sequence.</typeparam>
    /// <param name="connection">DB connection to use.</param>
    /// <param name="sql">SQL text of the command. May or may not end with semicolon.</param>
    /// <param name="param">Optional parameters for the command.</param>
    public static IBatchedValue<T?> QuerySingleOrDefaultBatched<T>(this IDbConnection connection, string sql, object? param = null)
    {
        return globalBatcher.QuerySingleOrDefaultBatched<T>(connection, sql, param);
    }

    /// <summary>
    /// Batches a DB command to INSERT/UPDATE/DELETE etc and returns the number of affected rows by all <see cref="ExecuteBatched"/> commands in the batch.
    /// </summary>
    /// <typeparam name="T">Type of element in the sequence.</typeparam>
    /// <param name="connection">DB connection to use.</param>
    /// <param name="sql">SQL text of the command. May or may not end with semicolon.</param>
    /// <param name="param">Optional parameters for the command.</param>
    public static IBatchedValue<int> ExecuteBatched(this IDbConnection connection, string sql, object? param = null)
    {
        return globalBatcher.ExecuteBatched(connection, sql, param);
    }
}
