using System.Data;
using System.Diagnostics;

namespace EdShel.DapperBatcher.Batching;

internal class NonQueryValue(
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
            await context!.FlushBatchAsync(shouldAsync: true, cancellationToken);
            Debug.Assert(resultReceived, "Batch was executed by this item haven't received the result.");
        }

        return result;
    }

    public int GetValue()
    {
        if (!resultReceived)
        {
            context!.FlushBatchAsync(shouldAsync: false, CancellationToken.None).GetAwaiter().GetResult();
            Debug.Assert(resultReceived, "Batch was executed by this item haven't received the result.");
        }

        return result;
    }
}
