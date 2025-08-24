namespace EdShel.DapperBatcher;

[Serializable]
public class SqlBatchingException : InvalidOperationException
{
    public SqlBatchingException() { }
    public SqlBatchingException(string message) : base(message) { }
    public SqlBatchingException(string message, Exception inner) : base(message, inner) { }
}