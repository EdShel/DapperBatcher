using EdShel.DapperBatcher.Tests.Helpers;

namespace EdShel.DapperBatcher.Tests.BatcherExtensionsTests;

public class QueryFirstBatchedTests
{
    [Fact]
    public void QueryFirstBatched_WhenSelectingOneCatWithoutParameters_ReturnsCatWithMinId()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat ORDER BY Id LIMIT 1");
        var cat = batch.GetValue();

        // Assert
        Assert.NotNull(cat);
        Assert.Equal(1, cat.Id);
    }

    [Fact]
    public void QueryFirstBatched_WhenSelectingExistingCatById_ReturnsObject()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
        var cat = batch.GetValue();

        // Assert
        Assert.NotNull(cat);
        Assert.Equal(1, cat.Id);
    }

    [Fact]
    public void QueryFirstBatched_WhenSelectingInt_ReturnsInt()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QueryFirstBatched<int>("SELECT Id FROM Cat WHERE Id = $Id", new { Id = 1 });
        var id = batch.GetValue();

        // Assert
        Assert.Equal(1, id);
    }

    [Fact]
    public void QueryFirstBatched_WhenSelectingNullableInt_ReturnsIntWithValue()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QueryFirstBatched<int?>("SELECT Id FROM Cat WHERE Id = $Id", new { Id = 1 });
        var id = batch.GetValue();

        // Assert
        Assert.True(id.HasValue);
        Assert.Equal(1, id.Value);
    }

    [Fact]
    public void QueryFirstBatched_WhenSelectingMissingCatById_Throws()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 222 });

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(() => batch.GetValue());
        Assert.Contains("Sequence contains no elements", exception.Message);
    }

    [Fact]
    public void QueryFirstBatched_WhenSelectingMissingNullableInt_Throws()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QueryFirstBatched<int?>("SELECT Id FROM Cat WHERE Id = $Id", new { Id = 222 });

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(() => batch.GetValue());
        Assert.Contains("Sequence contains no elements", exception.Message);
    }

    [Fact]
    public void QueryFirstBatched_WhenSelectingMoreThanOne_ReturnsFirst()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat ORDER BY Id");
        var cat = batch.GetValue();

        // Assert
        Assert.NotNull(cat);
        Assert.Equal(1, cat.Id);
    }

    [Fact]
    public void QueryFirstBatched_WhenSelectingTwoCatsInBatch_ReturnsRespectiveIds()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch1 = db.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
        var batch2 = db.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 2 });
        var cat1 = batch1.GetValue();
        var cat2 = batch2.GetValue();

        // Assert
        Assert.NotNull(cat1);
        Assert.Equal(1, cat1.Id);
        Assert.NotNull(cat2);
        Assert.Equal(2, cat2.Id);
    }

    [Fact]
    public void QueryFirstBatched_WhenSelectingTwoCatsInBatchButOneIsMissing_ReturnsForOneAndThrowsForTheOther()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch1 = db.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
        var batch2 = db.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 222 });
        var cat1 = batch1.GetValue();
        var cat2Action = () => batch2.GetValue();

        // Assert
        Assert.NotNull(cat1);
        Assert.Equal(1, cat1.Id);
        var exception = Assert.Throws<InvalidOperationException>(cat2Action);
        Assert.Contains("Sequence contains no elements", exception.Message);
    }
}
