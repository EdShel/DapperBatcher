using System.Data;
using System.Diagnostics;
using Dapper;

namespace EdShel.DapperBatcher;

internal class FirstOrSingleValue<T>(
    ConnectionContext? context,
    FirstOrSingleKind kind
) : IBatchedValue<T>, IBatchItem
{
    private List<T>? result = null;

    void IBatchItem.ReceiveResult(IDataReader dataReader)
    {
        if (result != null)
        {
            throw new InvalidOperationException("Result has already been received.");
        }

        result = SqlMapper.Parse<T>(dataReader).ToList();
        context = null; // Allow DB connection to be GC'ed
    }

    public async Task<T> GetValueAsync(CancellationToken cancellationToken = default)
    {
        if (result == null)
        {
            await context!.FlushBatchAsync(shouldAsync: true, cancellationToken);
            Debug.Assert(result != null, "Batch was executed by this item haven't received the result.");
        }

        return GetValueWithDefaultsOrThrow();
    }

    private T GetValueWithDefaultsOrThrow()
    {
        return kind switch
        {
            FirstOrSingleKind.First => result!.First(),
            FirstOrSingleKind.FirstOrDefault => result!.FirstOrDefault()!,
            FirstOrSingleKind.Single => result!.Single(),
            FirstOrSingleKind.SingleOrDefault => result!.SingleOrDefault()!,
            _ => throw new InvalidOperationException($"Unknown {nameof(FirstOrSingleKind)}: '{kind}'."),
        };
    }

    public T GetValue()
    {
        if (result == null)
        {
            context!.FlushBatchAsync(shouldAsync: false, CancellationToken.None).GetAwaiter().GetResult();
            Debug.Assert(result != null, "Batch was executed by this item haven't received the result.");
        }

        return GetValueWithDefaultsOrThrow();
    }
}
