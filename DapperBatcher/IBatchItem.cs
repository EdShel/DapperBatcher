using System.Data;

namespace EdShel.DapperBatcher;

internal interface IBatchItem
{
    void ReceiveResult(IDataReader dataReader);
}
