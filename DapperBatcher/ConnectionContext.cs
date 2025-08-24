using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Text;
using Dapper;

namespace EdShel.DapperBatcher;

internal class ConnectionContext(
    IDbConnection realConnection // TODO: check for memory leak (don't know if this holds the reference)
)
{
    internal readonly List<IBatchItem> BatchedCommands = new();

    internal async Task FlushBatchAsync(CancellationToken cancellationToken)
    {
        // TODO: don't reopen conenction if it's already open
        realConnection.Open();
        IDbCommand realCommand = CreateRealCommand();
        // TODO: cast to DbCommand and execute async if needed
        var dataReader = realCommand is DbCommand dbCommand
            ? await dbCommand.ExecuteReaderAsync(cancellationToken)
            : realCommand.ExecuteReader();
        int commandIndex = 0;
        do
        {
            Debug.Assert(commandIndex < BatchedCommands.Count);
            var deserializedResult = SqlMapper.Parse(dataReader, BatchedCommands[commandIndex].ResultType);
            BatchedCommands[commandIndex].ReceiveResult(deserializedResult.FirstOrDefault());
            commandIndex++;
        } while (dataReader.NextResult());
    }

    private IDbCommand CreateRealCommand()
    {
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
        return realCommand;
    }
}
