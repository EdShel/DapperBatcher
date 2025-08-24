using System.Runtime.CompilerServices;
using EdShel.DapperBatcher.Tests.Helpers;

namespace EdShel.DapperBatcher.Tests.BatcherExtensionsTests;

public class MemoryLeakTests
{
    [Fact]
    public void QueryFirstBatched_WhenDestroyingConnection_ContextsAreCleared()
    {
        // Arrange
        var batcher = new DapperCommandsBatcher();
        const int connectionsCount = 10;

        // Act
        for (int i = 0; i < connectionsCount; i++)
        {
            using var db = TestDbConnectionFactory.CreateInMemoryEmptyDB();
            var batch = batcher.QueryFirstBatched<int>(db, "SELECT 1");
            _ = batch.GetValue();
        }

        GC.Collect();
        GC.WaitForPendingFinalizers();

        int actualContextsCount = batcher.Contexts.Count();

        // Assert
        Assert.True(actualContextsCount < connectionsCount);
    }
}
