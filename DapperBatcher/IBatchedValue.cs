namespace EdShel.DapperBatcher;

public interface IBatchedValue<T>
{
    T GetValue();
    Task<T> GetValueAsync(CancellationToken cancellationToken = default);
}
