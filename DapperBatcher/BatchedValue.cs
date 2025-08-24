namespace EdShel.DapperBatcher;

internal class BatchedValue<T>(
    ConnectionContext context,
    ProxyCommand command,
    Type resultType
) : IBatchedValue<T>, IBatchItem
{
    private bool resultReceived = false;
    private T? result = default;

    ProxyCommand IBatchItem.Command => command;
    Type IBatchItem.ResultType => resultType;

    void IBatchItem.ReceiveResult(object? value)
    {
        if (resultReceived)
        {
            throw new InvalidOperationException("Result has already been received.");
        }
        if (value is not T)
        {
            throw new InvalidCastException($"Expected value of type {typeof(T)}, but received {value?.GetType()}.");
        }
        resultReceived = true;
        result = (T)value;
    }

    public async Task<T> GetValueAsync(CancellationToken cancellationToken = default)
    {
        if (!resultReceived)
        {
            await context.FlushBatchAsync(cancellationToken);

            if (!resultReceived)
            {
                throw new InvalidOperationException("Batch was executed by this item haven't received the result.");
            }
        }
        return result!;
    }

    public T GetValue()
    {
        return GetValueAsync(CancellationToken.None).GetAwaiter().GetResult();
    }
}
