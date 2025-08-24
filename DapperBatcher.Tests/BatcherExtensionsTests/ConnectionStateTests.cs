using System.Data;
using EdShel.DapperBatcher.Tests.Helpers;

namespace EdShel.DapperBatcher.Tests.BatcherExtensionsTests;

public class ConnectionStateTests
{
    [Fact]
    public void BatcherExtensions_WhenConnectionIsOpen_DoesntClose()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        ConnectionState initialState = db.State;
        var batch = db.QueryFirstBatched<bool>("SELECT EXISTS(SELECT 1 FROM Cat)");
        bool anyCatExists = batch.GetValue();
        ConnectionState finalState = db.State;

        // Assert
        Assert.Equal(ConnectionState.Open, initialState);
        Assert.True(anyCatExists);
        Assert.Equal(ConnectionState.Open, finalState);
    }

    [Fact]
    public void BatcherExtensions_WhenConnectionIsClosed_OpensAndCloses()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryEmptyDB();

        // Act
        ConnectionState initialState = db.State;
        var batch = db.QueryFirstBatched<int>("SELECT 1");
        ConnectionState beforeFlushingBatchState = db.State;
        _ = batch.GetValue();
        ConnectionState finalState = db.State;

        // Assert
        Assert.Equal(ConnectionState.Closed, initialState);
        Assert.Equal(ConnectionState.Closed, beforeFlushingBatchState);
        Assert.Equal(ConnectionState.Closed, finalState);
    }
}
