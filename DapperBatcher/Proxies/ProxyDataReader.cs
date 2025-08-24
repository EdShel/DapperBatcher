using System.Data;

namespace EdShel.DapperBatcher.Proxies;

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
    }

    public void Dispose()
    {
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
