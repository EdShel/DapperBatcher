namespace EdShel.DapperBatcher;

internal interface IBatchItem
{
    Type ResultType { get; }
    void ReceiveResult(object? value);
    ProxyCommand Command { get; }
}
