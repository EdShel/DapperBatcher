using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace EdShel.DapperBatcher.Proxies;

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
        throw new NotImplementedException();
    }

    public IDbTransaction BeginTransaction(IsolationLevel il)
    {
        throw new NotImplementedException();
    }

    public void ChangeDatabase(string databaseName)
    {
    }

    public void Close()
    {
    }

    public IDbCommand CreateCommand()
    {
        var command = new ProxyCommand(this);
        CreatedCommands.Add(command);
        return command;
    }

    public void Dispose() { }

    public void Open() { }
}
