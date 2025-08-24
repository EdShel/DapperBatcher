using System.Data;
using System.Diagnostics;
using Dapper;

namespace EdShel.DapperBatcher;

internal class MultiValue<T>(
    ConnectionContext? context
) : IBatchedValue<IEnumerable<T>>, IBatchItem
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

    public async Task<IEnumerable<T>> GetValueAsync(CancellationToken cancellationToken = default)
    {
        if (result == null)
        {
            await context!.FlushBatchAsync(shouldAsync: true, cancellationToken);
            Debug.Assert(result != null, "Batch was executed by this item haven't received the result.");
        }

        return result!;
    }

    public IEnumerable<T> GetValue()
    {
        if (result == null)
        {
            context!.FlushBatchAsync(shouldAsync: false, CancellationToken.None).GetAwaiter().GetResult();
            Debug.Assert(result != null, "Batch was executed by this item haven't received the result.");
        }

        return result!;
    }
}
