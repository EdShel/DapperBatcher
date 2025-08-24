using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace EdShel.DapperBatcher.Proxies;

internal class ProxyParameter : IDbDataParameter
{
    public DbType DbType { get; set; }
    public ParameterDirection Direction { get; set; } = ParameterDirection.Input;
    public bool IsNullable => true;
    [AllowNull]
    public string ParameterName { get; set; }
    [AllowNull]
    public string SourceColumn { get; set; }
    public DataRowVersion SourceVersion { get; set; } = DataRowVersion.Current;
    public object? Value { get; set; }
    public byte Precision { get; set; }
    public byte Scale { get; set; }
    public int Size { get; set; }

    public void CopyTo(IDbDataParameter target)
    {
        target.ParameterName = ParameterName;
        target.DbType = DbType;
        target.Direction = Direction;
        target.SourceColumn = SourceColumn;
        target.SourceVersion = SourceVersion;
        target.Value = Value;
        target.Precision = Precision;
        target.Scale = Scale;
        target.Size = Size;
    }
}
