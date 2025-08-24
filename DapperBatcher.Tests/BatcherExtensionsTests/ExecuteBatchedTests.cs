using EdShel.DapperBatcher.Exceptions;
using EdShel.DapperBatcher.Tests.Helpers;

namespace EdShel.DapperBatcher.Tests.BatcherExtensionsTests;

public class ExecuteBatchedTests
{
    [Fact]
    public void ExecuteBatched_WhenSingleSelectOperation_ReturnsMinus1()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.ExecuteBatched("SELECT 1");
        var value = batch.GetValue();

        // Assert
        Assert.Equal(-1, value);
    }

    [Fact]
    public void QueryFirstBatched_WhenInsertOperation_Throws()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QueryFirstBatched<int>("INSERT INTO Cat (Id, Name, CoatColor) VALUES ($Id, $Name, $CoatColor);", new Cat { Id = 6, Name = "Snowball", CoatColor = "White" });

        // Assert
        var exception = Assert.Throws<SqlBatchingException>(() => batch.GetValue());
        Assert.Contains("Batch returned RecordsAffected even though there was no ExecuteBatched command", exception.Message);
    }

    [Fact]
    public void ExecuteBatched_WhenInserted1Row_Returns1()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.ExecuteBatched("INSERT INTO Cat (Id, Name, CoatColor) VALUES ($Id, $Name, $CoatColor);", new Cat { Id = 6, Name = "Snowball", CoatColor = "White" });
        int rowsAffected = batch.GetValue();

        // Assert
        Assert.Equal(1, rowsAffected);
    }

    [Fact]
    public void ExecuteBatched_WhenInserted2Rows_Returns2ForAll()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch1 = db.ExecuteBatched("INSERT INTO Cat (Id, Name, CoatColor) VALUES ($Id, $Name, $CoatColor);", new Cat { Id = 6, Name = "Snowball", CoatColor = "White" });
        var batch2 = db.ExecuteBatched("INSERT INTO Cat (Id, Name, CoatColor) VALUES ($Id, $Name, $CoatColor);", new Cat { Id = 7, Name = "Snowball 2", CoatColor = "Black" });
        int rowsAffected1 = batch1.GetValue();
        int rowsAffected2 = batch2.GetValue();

        // Assert
        Assert.Equal(2, rowsAffected1);
        Assert.Equal(2, rowsAffected2);
    }

    [Fact]
    public void ExecuteBatched_WhenDeleteAll_ReturnsNumberOfRows()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.ExecuteBatched("DELETE FROM Cat;");
        int rowsAffected = batch.GetValue();

        // Assert
        Assert.Equal(5, rowsAffected);
    }

    [Fact]
    public void ExecuteBatched_WhenUpdateAll_ReturnsNumberOfRows()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.ExecuteBatched("UPDATE Cat SET Name = @Name;", new { Name = "Sold" });
        int rowsAffected = batch.GetValue();

        // Assert
        Assert.Equal(5, rowsAffected);
    }
}
