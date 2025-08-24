using System.Data;

namespace EdShel.DapperBatcher;

internal class ProxyDbConnection(IDbConnection inner) : IDbConnection
{
    internal List<ProxyCommand> CreatedCommands = new();

    public string ConnectionString
    {
        get
        {
            Console.WriteLine("Get ConnectionString DapperDbConnectionProxy");
            return inner.ConnectionString;
        }
        set
        {
            Console.WriteLine("Set ConnectionString DapperDbConnectionProxy");
            Console.WriteLine($"Proxy setting ConnectionString to: {value}");
            inner.ConnectionString = value;
        }
    }

    public int ConnectionTimeout
    {
        get
        {
            Console.WriteLine("Get ConnectionTimeout DapperDbConnectionProxy");
            return inner.ConnectionTimeout;
        }
    }

    public string Database
    {
        get
        {
            Console.WriteLine("Get Database DapperDbConnectionProxy");
            return inner.Database;
        }
    }

    public ConnectionState State
    {
        get
        {
            Console.WriteLine("Get State DapperDbConnectionProxy");
            return inner.State;
        }
    }

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
