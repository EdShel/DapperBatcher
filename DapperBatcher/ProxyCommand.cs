using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace EdShel.DapperBatcher;

internal class ProxyCommand(IDbConnection connection) : IDbCommand
{
    [AllowNull]
    public string CommandText { get; set; }

    private ProxyParametersCollection parametersCollection = new();

    #region Placeholders
    public int CommandTimeout
    {
        get
        {
            Console.WriteLine("Get CommandTimeout DapperDbCommandProxy");
            return 0;
        }
        set
        {
            Console.WriteLine("Set CommandTimeout DapperDbCommandProxy");
        }
    }
    public CommandType CommandType
    {
        get
        {
            Console.WriteLine("Get CommandType DapperDbCommandProxy");
            return CommandType.Text;
        }
        set
        {
            Console.WriteLine("Set CommandType DapperDbCommandProxy");
        }
    }
    public IDbTransaction? Transaction
    {
        get
        {
            Console.WriteLine("Get Transaction DapperDbCommandProxy");
            return null;
        }
        set
        {
            Console.WriteLine("Set Transaction DapperDbCommandProxy");
        }
    }
    public UpdateRowSource UpdatedRowSource
    {
        get
        {
            Console.WriteLine("Get UpdatedRowSource DapperDbCommandProxy");
            return UpdateRowSource.None;
        }
        set
        {
            Console.WriteLine("Set UpdatedRowSource DapperDbCommandProxy");
        }
    }

    #endregion

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
        Console.WriteLine("Cancel DapperDbCommandProxy");
    }

    public IDbDataParameter CreateParameter()
    {
        Console.WriteLine("CreateParameter DapperDbCommandProxy");
        return new ProxyParameter();
    }

    public void Dispose()
    {
        Console.WriteLine("Dispose DapperDbCommandProxy");
    }

    public int ExecuteNonQuery()
    {
        Console.WriteLine("ExecuteNonQuery DapperDbCommandProxy");
        return 0;
    }

    public IDataReader ExecuteReader()
    {
        Console.WriteLine("ExecuteReader DapperDbCommandProxy");
        return new ProxyDataReader();
    }

    public IDataReader ExecuteReader(CommandBehavior behavior)
    {
        Console.WriteLine("ExecuteReader(CommandBehavior) DapperDbCommandProxy");
        return new ProxyDataReader();
    }

    public object? ExecuteScalar()
    {
        Console.WriteLine("ExecuteScalar DapperDbCommandProxy");
        return null;
    }

    public void Prepare()
    {
        Console.WriteLine("Prepare DapperDbCommandProxy");
    }
}
