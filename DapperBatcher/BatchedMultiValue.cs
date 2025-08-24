using System.Data;
using Dapper;

namespace EdShel.DapperBatcher;

internal class BatchedMultiValue<T>(
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
            await context!.FlushBatchAsync(cancellationToken);

            if (result == null)
            {
                throw new InvalidOperationException("Batch was executed by this item haven't received the result.");
            }
        }

        return result!;
    }

    public IEnumerable<T> GetValue()
    {
        return GetValueAsync().GetAwaiter().GetResult();
    }
}
