using DapperBatcher.Tests.Helpers;
using EdShel.DapperBatcher;

namespace DapperBatcher.Tests.BatcherExtensionsTests;

public class QuerySingleOrDefaultBatchedTests
{
    [Fact]
    public void QuerySingleOrDefaultBatched_WhenSelectingOneCatWithoutParameters_ReturnsCatWithMinId()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QuerySingleOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat ORDER BY Id LIMIT 1");
        var cat = batch.GetValue();

        // Assert
        Assert.NotNull(cat);
        Assert.Equal(1, cat.Id);
    }

    [Fact]
    public void QuerySingleOrDefaultBatched_WhenSelectingExistingCatById_ReturnsObject()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QuerySingleOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
        var cat = batch.GetValue();

        // Assert
        Assert.NotNull(cat);
        Assert.Equal(1, cat.Id);
    }

    [Fact]
    public void QuerySingleOrDefaultBatched_WhenSelectingInt_ReturnsInt()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QuerySingleOrDefaultBatched<int>("SELECT Id FROM Cat WHERE Id = $Id", new { Id = 1 });
        var id = batch.GetValue();

        // Assert
        Assert.Equal(1, id);
    }

    [Fact]
    public void QuerySingleOrDefaultBatched_WhenSelectingNullableInt_ReturnsIntWithValue()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QuerySingleOrDefaultBatched<int?>("SELECT Id FROM Cat WHERE Id = $Id", new { Id = 1 });
        var id = batch.GetValue();

        // Assert
        Assert.True(id.HasValue);
        Assert.Equal(1, id.Value);
    }

    [Fact]
    public void QuerySingleOrDefaultBatched_WhenSelectingMissingCatById_ReturnsNull()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QuerySingleOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 222 });
        var cat = batch.GetValue();

        // Assert
        Assert.Null(cat);
    }

    [Fact]
    public void QuerySingleOrDefaultBatched_WhenSelectingMissingNullableInt_ReturnsNullValue()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QuerySingleOrDefaultBatched<int?>("SELECT Id FROM Cat WHERE Id = $Id", new { Id = 222 });
        var id = batch.GetValue();

        // Assert
        Assert.False(id.HasValue);
    }

    [Fact]
    public void QuerySingleOrDefaultBatched_WhenSelectingMoreThanOne_Throws()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QuerySingleOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat ORDER BY Id");
        var catAction = () => batch.GetValue();

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(catAction);
        Assert.Contains("Sequence contains more than one element", exception.Message);
    }

    [Fact]
    public void QuerySingleOrDefaultBatched_WhenTwoQueriesAndOneHas2Elements_ThrowsForOneButNotTheOther()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batchOf2 = db.QuerySingleOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id1 OR Id = $Id2", new { Id1 = 1, Id2 = 2 });
        var batchOf1 = db.QuerySingleOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });

        var catOf1 = batchOf1.GetValue();
        var catOf2Action = () => batchOf2.GetValue();

        // Assert
        Assert.NotNull(catOf1);
        var exception = Assert.Throws<InvalidOperationException>(catOf2Action);
        Assert.Contains("Sequence contains more than one element", exception.Message);
    }
}
