using DapperBatcher.Tests.Helpers;
using EdShel.DapperBatcher;

namespace DapperBatcher.Tests.BatcherExtensionsTests;

public class QueryFirstOrDefaultBatchedTests
{
    [Fact]
    public void QueryFirstOrDefaultBatched_WhenSelectingOneCatWithoutParameters_ReturnsCatWithMinId()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat ORDER BY Id LIMIT 1");
        var cat = batch.GetValue();

        // Assert
        Assert.NotNull(cat);
        Assert.Equal(1, cat.Id);
    }

    [Fact]
    public void QueryFirstOrDefaultBatched_WhenSelectingExistingCatById_ReturnsObject()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
        var cat = batch.GetValue();

        // Assert
        Assert.NotNull(cat);
        Assert.Equal(1, cat.Id);
    }

    [Fact]
    public void QueryFirstOrDefaultBatched_WhenSelectingInt_ReturnsInt()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QueryFirstOrDefaultBatched<int>("SELECT Id FROM Cat WHERE Id = $Id", new { Id = 1 });
        var id = batch.GetValue();

        // Assert
        Assert.Equal(1, id);
    }

    [Fact]
    public void QueryFirstOrDefaultBatched_WhenSelectingNullableInt_ReturnsIntWithValue()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QueryFirstOrDefaultBatched<int?>("SELECT Id FROM Cat WHERE Id = $Id", new { Id = 1 });
        var id = batch.GetValue();

        // Assert
        Assert.True(id.HasValue);
        Assert.Equal(1, id.Value);
    }

    [Fact]
    public void QueryFirstOrDefaultBatched_WhenSelectingMissingCatById_ReturnsNull()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 222 });
        var cat = batch.GetValue();

        // Assert
        Assert.Null(cat);
    }

    [Fact]
    public void QueryFirstOrDefaultBatched_WhenSelectingMissingNullableInt_ReturnsNullValue()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QueryFirstOrDefaultBatched<int?>("SELECT Id FROM Cat WHERE Id = $Id", new { Id = 222 });
        var id = batch.GetValue();

        // Assert
        Assert.False(id.HasValue);
    }

    [Fact]
    public void QueryFirstOrDefaultBatched_WhenSelectingMoreThanOne_ReturnsFirst()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch = db.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat ORDER BY Id");
        var cat = batch.GetValue();

        // Assert
        Assert.NotNull(cat);
        Assert.Equal(1, cat.Id);
    }

    [Fact]
    public void QueryFirstOrDefaultBatched_WhenSelectingTwoCatsInBatch_ReturnsRespectiveIds()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch1 = db.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
        var batch2 = db.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 2 });
        var cat1 = batch1.GetValue();
        var cat2 = batch2.GetValue();

        // Assert
        Assert.NotNull(cat1);
        Assert.Equal(1, cat1.Id);
        Assert.NotNull(cat2);
        Assert.Equal(2, cat2.Id);
    }
}
