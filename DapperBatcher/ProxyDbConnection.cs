using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace EdShel.DapperBatcher;

internal class ProxyDbConnection : IDbConnection
{
    internal List<ProxyCommand> CreatedCommands = new();

    [AllowNull]
    public string ConnectionString
    {
        get => ":proxy:";
        set { }
    }

    public int ConnectionTimeout => 15;

    public string Database => string.Empty;

    public ConnectionState State => ConnectionState.Open;
    public IDbTransaction BeginTransaction()
    {
        Console.WriteLine("BeginTransaction DapperDbConnectionProxy");
        throw new NotImplementedException();
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        Console.WriteLine("BeginTransaction(il) DapperDbConnectionProxy");
        throw new NotImplementedException();
    }

    public void ChangeDatabase(string databaseName)
    {
        Console.WriteLine("ChangeDatabase DapperDbConnectionProxy");
    }

    public void Close()
    {
        Console.WriteLine("Close DapperDbConnectionProxy");
    }

    public IDbCommand CreateCommand()
    {
        Console.WriteLine("CreateCommand DapperDbConnectionProxy");

        var command = new ProxyCommand(this);
        CreatedCommands.Add(command);
        return command;
    }

    public void Dispose() { }

    public void Open() { }
}
