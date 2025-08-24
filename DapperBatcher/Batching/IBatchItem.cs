using System.Data;

namespace EdShel.DapperBatcher.Batching;

internal interface IBatchItem
{
    void ReceiveResult(IDataReader dataReader);
}
