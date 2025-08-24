using System.Data;

namespace EdShel.DapperBatcher;

internal class BatchedNonQueryValue(
    ConnectionContext? context
) : IBatchedValue<int>, IBatchItem
{
    private bool resultReceived = false;
    private int result;

    void IBatchItem.ReceiveResult(IDataReader dataReader)
    {
        if (resultReceived)
        {
            throw new InvalidOperationException("Result has already been received.");
        }
        resultReceived = true;

        result = dataReader.RecordsAffected;
        context = null; // Allow DB connection to be GC'ed
    }

    public async Task<int> GetValueAsync(CancellationToken cancellationToken = default)
    {
        if (!resultReceived)
        {
            await context!.FlushBatchAsync(cancellationToken);

            if (!resultReceived)
            {
                throw new InvalidOperationException("Batch was executed by this item haven't received the result.");
            }
        }

        return result;
    }

    public int GetValue()
    {
        return GetValueAsync().GetAwaiter().GetResult();
    }
}
