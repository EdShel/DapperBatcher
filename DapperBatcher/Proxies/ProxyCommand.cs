using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace EdShel.DapperBatcher.Proxies;

internal class ProxyCommand(IDbConnection connection) : IDbCommand
{
    [AllowNull]
    public string CommandText { get; set; }

    private ProxyParametersCollection parametersCollection = new();

    public int CommandTimeout
    {
        get => 0;
        set { }
    }
    public CommandType CommandType
    {
        get => CommandType.Text;
        set { }
    }
    public IDbTransaction? Transaction
    {
        get => null;
        set { }
    }
    public UpdateRowSource UpdatedRowSource
    {
        get => UpdateRowSource.None;
        set { }
    }

    public IDbConnection? Connection
    {
        get => connection;
        set
        {
            connection = value!;
        }
    }

    public IDataParameterCollection Parameters => parametersCollection;
    internal ProxyParametersCollection ParametersSafe => parametersCollection;

    public void Cancel()
    {
    }

    public IDbDataParameter CreateParameter()
    {
        return new ProxyParameter();
    }

    public void Dispose()
    {
    }

    public int ExecuteNonQuery()
    {
        return 0;
    }

    public IDataReader ExecuteReader()
    {
        return new ProxyDataReader();
    }

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
        return new ProxyDataReader();
    }

    public object? ExecuteScalar()
    {
        return null;
    }

    public void Prepare()
    {
    }
}
