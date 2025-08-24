namespace EdShel.DapperBatcher;

/// <summary>
/// Lazy accessor to the value returned by a batched query.
/// </summary>
/// <typeparam name="T">Query result type.</typeparam>
public interface IBatchedValue<T>
{
    /// <summary>
    /// Returns the query result, executing the entire batch if needed.
    /// </summary>
    T GetValue();

    /// <summary>
    /// Asynchronously returns the query result, executing the entire batch if needed.
    /// </summary>
    Task<T> GetValueAsync(CancellationToken cancellationToken = default);
}
