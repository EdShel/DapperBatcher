using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace EdShel.DapperBatcher.Proxies;

internal class ProxyParameter : IDbDataParameter
{
    private DbType _dbType;
    public DbType DbType
    {
        get
        {
            Console.WriteLine("Get DbType DapperDbDataParameterProxy");
            return _dbType;
        }
        set
        {
            Console.WriteLine($"Set DbType DapperDbDataParameterProxy: {value}");
            _dbType = value;
        }
    }

    private ParameterDirection _direction = ParameterDirection.Input;
    public ParameterDirection Direction
    {
        get
        {
            Console.WriteLine("Get Direction DapperDbDataParameterProxy");
            return _direction;
        }
        set
        {
            Console.WriteLine($"Set Direction DapperDbDataParameterProxy: {value}");
            _direction = value;
        }
    }

    public bool IsNullable
    {
        get
        {
            Console.WriteLine("Get IsNullable DapperDbDataParameterProxy");
            return true;
        }
    }

    private string _parameterName = string.Empty;
    [AllowNull]
    public string ParameterName
    {
        get
        {
            Console.WriteLine("Get ParameterName DapperDbDataParameterProxy");
            return _parameterName;
        }
        set
        {
            Console.WriteLine($"Set ParameterName DapperDbDataParameterProxy: {value}");
            _parameterName = value!;
        }
    }

    private string _sourceColumn = string.Empty;
    [AllowNull]
    public string SourceColumn
    {
        get
        {
            Console.WriteLine("Get SourceColumn DapperDbDataParameterProxy");
            return _sourceColumn;
        }
        set
        {
            Console.WriteLine($"Set SourceColumn DapperDbDataParameterProxy: {value}");
            _sourceColumn = value!;
        }
    }

    private DataRowVersion _sourceVersion = DataRowVersion.Current;
    public DataRowVersion SourceVersion
    {
        get
        {
            Console.WriteLine("Get SourceVersion DapperDbDataParameterProxy");
            return _sourceVersion;
        }
        set
        {
            Console.WriteLine($"Set SourceVersion DapperDbDataParameterProxy: {value}");
            _sourceVersion = value;
        }
    }

    private object? _value;
    public object? Value
    {
        get
        {
            Console.WriteLine("Get Value DapperDbDataParameterProxy");
            return _value;
        }
        set
        {
            Console.WriteLine($"Set Value DapperDbDataParameterProxy: {value}");
            _value = value;
        }
    }

    private byte _precision;
    public byte Precision
    {
        get
        {
            Console.WriteLine("Get Precision DapperDbDataParameterProxy");
            return _precision;
        }
        set
        {
            Console.WriteLine($"Set Precision DapperDbDataParameterProxy: {value}");
            _precision = value;
        }
    }

    private byte _scale;
    public byte Scale
    {
        get
        {
            Console.WriteLine("Get Scale DapperDbDataParameterProxy");
            return _scale;
        }
        set
        {
            Console.WriteLine($"Set Scale DapperDbDataParameterProxy: {value}");
            _scale = value;
        }
    }

    private int _size;
    public int Size
    {
        get
        {
            Console.WriteLine("Get Size DapperDbDataParameterProxy");
            return _size;
        }
        set
        {
            Console.WriteLine($"Set Size DapperDbDataParameterProxy: {value}");
            _size = value;
        }
    }

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
