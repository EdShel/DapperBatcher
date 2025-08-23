using System.Data;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using Dapper;

namespace EdShel.DapperBatcher;

public static class DapperBatcherExtensions
{
    private static ConditionalWeakTable<IDbConnection, ConnectionContext> batchContexts = new();

    public static IBatchedValue<T> QueryFirstOrDefaultBatched<T>(this IDbConnection connection, string sql, object? param = null, IDbTransaction? transaction = null, int? commandTimeout = null, CommandType? commandType = null)
    {
        var connectionProxy = new ProxyDbConnection(connection);
        var result = connectionProxy.QueryFirstOrDefault(sql, param, transaction, commandTimeout, commandType);
        Debug.Assert(connectionProxy.CreatedCommands.Count == 1);
        var proxyCommand = connectionProxy.CreatedCommands[0];

        var context = batchContexts.GetValue(
            connection,
            static connection => new ConnectionContext(connection)
        );
        var item = new BatchedValue<T>(
            context,
            proxyCommand,
            typeof(T)
        );
        context.BatchedCommands.Add(item);

        return item;
    }

}



internal class ConnectionContext(
    IDbConnection realConnection // TODO: check for memory leak (don't know if this holds the reference)
)
{
    internal readonly List<IBatchItem> BatchedCommands = new();

    internal void FlushBatch()
    {
        // TODO: don't reopen conenction if it's already open
        realConnection.Open();
        var realCommand = realConnection.CreateCommand();
        var parametersHashmap = new Dictionary<string, IDbDataParameter>();

        int parametersCounter = 0;

        var sb = new StringBuilder();
        foreach (var batchItem in BatchedCommands)
        {
            ProxyCommand command = batchItem.Command;
            var parameterNamesToReplace = new Dictionary<string, string>();

            foreach (ProxyParameter commandParameter in command.ParametersSafe)
            {
                string paramName = commandParameter.ParameterName;
                // TODO: we might reuse the command if it has the same value, precision etc
                if (parametersHashmap.ContainsKey(paramName))
                {
                    const int maxParameters = 2048;
                    while (parametersCounter < maxParameters)
                    {
                        string newParamName = $"p{++parametersCounter}";
                        if (!parametersHashmap.ContainsKey(newParamName))
                        {
                            parameterNamesToReplace[paramName] = newParamName;
                            paramName = newParamName;
                            break;
                        }
                    }
                }

                var realParameter = realCommand.CreateParameter();
                commandParameter.CopyTo(realParameter);
                realParameter.ParameterName = paramName;
                realCommand.Parameters.Add(realParameter);

                parametersHashmap.Add(paramName, realParameter);
            }

            var commandSb = new StringBuilder(command.CommandText.AsSpan().Trim().ToString());
            foreach (var (oldName, newName) in parameterNamesToReplace)
            {
                foreach (string prefix in new[] { "@", ":", "$", "?" })
                {
                    commandSb.Replace(prefix + oldName, prefix + newName);
                }
            }
            sb.Append(commandSb);
            if (sb[^1] != ';')
            {
                sb.Append(';');
            }
        }

        realCommand.CommandText = sb.ToString();
        // TODO: cast to DbCommand and execute async if needed
        var dataReader = realCommand.ExecuteReader();
        int commandIndex = 0;
        do
        {
            Debug.Assert(commandIndex < BatchedCommands.Count);
            var deserializedResult = SqlMapper.Parse(dataReader, BatchedCommands[commandIndex].ResultType);
            BatchedCommands[commandIndex].ReceiveResult(deserializedResult.FirstOrDefault());
            commandIndex++;
        } while (dataReader.NextResult());
    }
}

internal class CommandsBatch
{
}

// internal class Command
// {

// }

internal interface IBatchItem
{
    Type ResultType { get; }
    void ReceiveResult(object? value);
    ProxyCommand Command { get; }
}

public interface IBatchedValue<T>
{
    T? GetValue();
    // Task<T> GetValueAsync(CancellationToken cancellationToken = default);
}

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

    public T? GetValue()
    {
        if (!resultReceived)
        {
            context.FlushBatch();

            if (!resultReceived)
            {
                throw new InvalidOperationException("Batch was executed by this item haven't received the result.");
            }
        }
        return result;
    }

    // public Task<T> GetValueAsync(CancellationToken cancellationToken = default)
    // {
    //     throw new NotImplementedException();
    // }
}


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

internal class ProxyDataReader : IDataReader
{
    public object this[int i] => null!;
    public object this[string name] => null!;

    public int Depth => 0;
    public bool IsClosed => false;
    public int RecordsAffected => 0;
    public int FieldCount => 0;

    public void Close()
    {
        Console.WriteLine("Close DapperDbDataReaderProxy");
    }

    public void Dispose()
    {
        Console.WriteLine("Dispose DapperDbDataReaderProxy");
    }

    public bool GetBoolean(int i) => false;
    public byte GetByte(int i) => 0;
    public long GetBytes(int i, long fieldOffset, byte[]? buffer, int bufferoffset, int length) => 0;
    public char GetChar(int i) => '\0';
    public long GetChars(int i, long fieldoffset, char[]? buffer, int bufferoffset, int length) => 0;
    public IDataReader GetData(int i) => this;
    public string GetDataTypeName(int i) => string.Empty;
    public DateTime GetDateTime(int i) => DateTime.MinValue;
    public decimal GetDecimal(int i) => 0;
    public double GetDouble(int i) => 0;
    public Type GetFieldType(int i) => typeof(object);
    public float GetFloat(int i) => 0;
    public Guid GetGuid(int i) => Guid.Empty;
    public short GetInt16(int i) => 0;
    public int GetInt32(int i) => 0;
    public long GetInt64(int i) => 0;
    public string GetName(int i) => string.Empty;
    public int GetOrdinal(string name) => 0;
    public DataTable GetSchemaTable() => new DataTable();
    public string GetString(int i) => string.Empty;
    public object GetValue(int i) => null!;
    public int GetValues(object[] values) => 0;
    public bool IsDBNull(int i) => true;
    public bool NextResult() => false;
    public bool Read()
    {
        return false;
    }
}
