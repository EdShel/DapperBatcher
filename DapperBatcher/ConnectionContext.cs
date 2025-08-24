using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;

namespace EdShel.DapperBatcher;

internal class ConnectionContext(
    IDbConnection realConnection
)
{
    /// <summary>
    /// List of commands created by Dapper.
    /// </summary>
    internal readonly List<ProxyCommand> BatchedCommands = new();

    /// <summary>
    /// List of value accessors given away to the user.
    /// Has the same length as <see cref="BatchedCommands"/>
    /// </summary>
    internal readonly List<IBatchItem> BatchedItems = new();

    internal void AddToBatch(IBatchItem item, ProxyCommand command)
    {
        BatchedCommands.Add(command);
        BatchedItems.Add(item);
    }

    internal async Task FlushBatchAsync(bool shouldAsync, CancellationToken cancellationToken)
    {
        bool wasClosed = realConnection.State == ConnectionState.Closed;
        if (wasClosed)
        {
            if (shouldAsync && realConnection is DbConnection dbConnection)
            {
                await dbConnection.OpenAsync(cancellationToken);
            }
            else
            {
                realConnection.Open();
            }
        }

        try
        {
            realConnection.Open();
            IDbCommand realCommand = CreateRealCommand();
            using var dataReader = shouldAsync && realCommand is DbCommand dbCommand
                ? await dbCommand.ExecuteReaderAsync(cancellationToken)
                : realCommand.ExecuteReader();
            int index = 0;
            bool hasNonQueryCommands = false;
            do
            {
                Debug.Assert(index < BatchedItems.Count);
                while (index < BatchedItems.Count && BatchedItems[index] is NonQueryValue)
                {
                    hasNonQueryCommands = true;
                    index++;
                }
                if (index >= BatchedItems.Count)
                {
                    // We've ran out of batched commands but there's still an unhandled result set.
                    // The following increment is needed for the error message below
                    index++;
                    break;
                }

                var batchedValue = BatchedItems[index];
                batchedValue.ReceiveResult(dataReader);
                index++;
            } while (dataReader.NextResult());

            if (index < BatchedItems.Count)
            {
                throw new SqlBatchingException(
                    "Batch returned fewer result sets than expected. Make sure all INSERT/UPDATE/DELETE commands are executed under ExecuteBatched.");
            }
            else if (index > BatchedItems.Count)
            {
                throw new SqlBatchingException(
                    "Batch returned more result sets than expected. This typically happens when multiple SELECT commands are queried under a single batched command. Try putting each SELECT in its own *Batched command.");
            }

            if (hasNonQueryCommands)
            {
                foreach (var batchedValue in BatchedItems)
                {
                    if (batchedValue is NonQueryValue)
                    {
                        batchedValue.ReceiveResult(dataReader);
                    }
                }
            }
        }
        finally
        {
            BatchedCommands.Clear();
            BatchedItems.Clear();
        }

        if (wasClosed && realConnection.State != ConnectionState.Closed)
        {
            realConnection.Close();
        }
    }

    private IDbCommand CreateRealCommand()
    {
        var realCommand = realConnection.CreateCommand();
        var parametersHashmap = new Dictionary<string, IDbDataParameter>();

        int parametersCounter = 0;

        var sb = new StringBuilder();
        foreach (ProxyCommand command in BatchedCommands)
        {
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
        return realCommand;
    }
}
