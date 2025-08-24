using EdShel.DapperBatcher.Tests.Helpers;

namespace EdShel.DapperBatcher.Tests.BatcherExtensionsTests;

public class QuerySingleBatchedTests
{
    [Fact]
    public void QuerySingleBatched_WhenSelectingOneCatWithoutParameters_ReturnsCatWithMinId()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QuerySingleBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat ORDER BY Id LIMIT 1");
        var cat = batch.GetValue();

        // Assert
        Assert.NotNull(cat);
        Assert.Equal(1, cat.Id);
    }

    [Fact]
    public void QuerySingleBatched_WhenSelectingExistingCatById_ReturnsObject()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QuerySingleBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
        var cat = batch.GetValue();

        // Assert
        Assert.NotNull(cat);
        Assert.Equal(1, cat.Id);
    }

    [Fact]
    public void QuerySingleBatched_WhenSelectingInt_ReturnsInt()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QuerySingleBatched<int>("SELECT Id FROM Cat WHERE Id = $Id", new { Id = 1 });
        var id = batch.GetValue();

        // Assert
        Assert.Equal(1, id);
    }

    [Fact]
    public void QuerySingleBatched_WhenSelectingNullableInt_ReturnsIntWithValue()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QuerySingleBatched<int?>("SELECT Id FROM Cat WHERE Id = $Id", new { Id = 1 });
        var id = batch.GetValue();

        // Assert
        Assert.True(id.HasValue);
        Assert.Equal(1, id.Value);
    }

    [Fact]
    public void QuerySingleBatched_WhenSelectingMissingCatById_Throws()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QuerySingleBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 222 });

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(() => batch.GetValue());
        Assert.Contains("Sequence contains no elements", exception.Message);
    }

    [Fact]
    public void QuerySingleBatched_WhenSelectingMissingNullableInt_Throws()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QuerySingleBatched<int?>("SELECT Id FROM Cat WHERE Id = $Id", new { Id = 222 });

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(() => batch.GetValue());
        Assert.Contains("Sequence contains no elements", exception.Message);
    }

    [Fact]
    public void QuerySingleBatched_WhenSelectingMoreThanOne_Throws()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch = db.QuerySingleBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat ORDER BY Id");

        // Assert
        var exception = Assert.Throws<InvalidOperationException>(() => batch.GetValue());
        Assert.Contains("Sequence contains more than one element", exception.Message);
    }

    [Fact]
    public void QuerySingleBatched_WhenSelectingTwoCatsInBatch_ReturnsRespectiveIds()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch1 = db.QuerySingleBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
        var batch2 = db.QuerySingleBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 2 });
        var cat1 = batch1.GetValue();
        var cat2 = batch2.GetValue();

        // Assert
        Assert.NotNull(cat1);
        Assert.Equal(1, cat1.Id);
        Assert.NotNull(cat2);
        Assert.Equal(2, cat2.Id);
    }

    [Fact]
    public void QuerySingleBatched_WhenSelectingTwoCatsInBatchButOneIsMissing_ReturnsForOneAndThrowsForTheOther()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch1 = db.QuerySingleBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
        var batch2 = db.QuerySingleBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 222 });
        var cat1 = batch1.GetValue();
        var cat2Action = () => batch2.GetValue();

        // Assert
        Assert.NotNull(cat1);
        Assert.Equal(1, cat1.Id);
        var exception = Assert.Throws<InvalidOperationException>(cat2Action);
        Assert.Contains("Sequence contains no elements", exception.Message);
    }
}
