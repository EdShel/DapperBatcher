using System.Data;
using Moq;

namespace EdShel.DapperBatcher.Tests.BatcherExtensionsTests;

public class BatchingTests
{
    [Fact]
    public void QueryBatched_WhenTwoQueriesInBatch_ExecutesCommandOnce()
    {
        // Arrange
        var readerMock = new Mock<IDataReader>();
        readerMock.Setup(r => r.Read()).Returns(false);
        readerMock.Setup(r => r.RecordsAffected).Returns(-1);
        readerMock.SetupSequence(r => r.NextResult()).Returns(true).Returns(false);

        var commandMock = new Mock<IDbCommand>();
        commandMock.Setup(com => com.ExecuteReader()).Returns(readerMock.Object);

        var connectionMock = new Mock<IDbConnection>();
        connectionMock.Setup(con => con.CreateCommand()).Returns(commandMock.Object);

        // Act
        IBatchedValue<IEnumerable<int>> batch1 = connectionMock.Object.QueryBatched<int>("SELECT 1");
        IBatchedValue<IEnumerable<int>> batch2 = connectionMock.Object.QueryBatched<int>("SELECT 2");
        _ = batch1.GetValue();

        // Assert
        commandMock.Verify(com => com.ExecuteReader(), Times.Once());
    }

    [Fact]
    public void QueryBatched_WhenNoGetValue_DoesntTouchConnection()
    {
        // Arrange
        var readerMock = new Mock<IDataReader>();
        readerMock.Setup(r => r.Read()).Returns(false);
        readerMock.Setup(r => r.RecordsAffected).Returns(-1);

        var commandMock = new Mock<IDbCommand>();
        commandMock.Setup(com => com.ExecuteReader()).Returns(readerMock.Object);

        var connectionMock = new Mock<IDbConnection>();
        connectionMock.Setup(con => con.CreateCommand()).Returns(commandMock.Object);

        // Act
        IBatchedValue<IEnumerable<int>> batch = connectionMock.Object.QueryBatched<int>("SELECT 1");

        // Assert
        connectionMock.VerifyNoOtherCalls();
    }

    [Fact]
    public void QueryBatched_WhenOneQuery_ExecutesCommandOnce()
    {
        // Arrange
        var readerMock = new Mock<IDataReader>();
        readerMock.Setup(r => r.Read()).Returns(false);
        readerMock.Setup(r => r.RecordsAffected).Returns(-1);

        var commandMock = new Mock<IDbCommand>();
        commandMock.Setup(com => com.ExecuteReader()).Returns(readerMock.Object);

        var connectionMock = new Mock<IDbConnection>();
        connectionMock.Setup(con => con.CreateCommand()).Returns(commandMock.Object);

        // Act
        IBatchedValue<IEnumerable<int>> batch = connectionMock.Object.QueryBatched<int>("SELECT 1");
        _ = batch.GetValue();

        // Assert
        commandMock.Verify(com => com.ExecuteReader(), Times.Once());
    }

    [Fact]
    public void QueryBatched_WhenTwoBatchesWithOneQueryEach_ExecutesCommandTwice()
    {
        // Arrange
        var readerMock = new Mock<IDataReader>();
        readerMock.Setup(r => r.Read()).Returns(false);
        readerMock.Setup(r => r.RecordsAffected).Returns(-1);

        var commandMock = new Mock<IDbCommand>();
        commandMock.Setup(com => com.ExecuteReader()).Returns(readerMock.Object);

        var connectionMock = new Mock<IDbConnection>();
        connectionMock.Setup(con => con.CreateCommand()).Returns(commandMock.Object);

        // Act
        IBatchedValue<IEnumerable<int>> batch1 = connectionMock.Object.QueryBatched<int>("SELECT 1");
        _ = batch1.GetValue();

        IBatchedValue<IEnumerable<int>> batch2 = connectionMock.Object.QueryBatched<int>("SELECT 1");
        _ = batch2.GetValue();

        // Assert
        commandMock.Verify(com => com.ExecuteReader(), Times.Exactly(2));
    }
}
