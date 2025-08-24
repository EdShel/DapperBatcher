using DapperBatcher.Tests.Helpers;
using EdShel.DapperBatcher;

namespace DapperBatcher.Tests.BatcherExtensionsTests;

public class GetValueTests
{
    [Fact]
    public void GetValue_WhenInsertAndSelect_OrderOfCallsShouldntMatter()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var selectBeforeBatch = db.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 6 });
        var insertBatch = db.ExecuteBatched("INSERT INTO Cat (Id, Name, CoatColor) VALUES ($Id, $Name, $CoatColor);", new Cat { Id = 6, Name = "Snowball", CoatColor = "White" });
        var selectAfterBatch = db.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 6 });

        int insertedRows = insertBatch.GetValue();
        var catBefore = selectBeforeBatch.GetValue();
        var catAfter = selectAfterBatch.GetValue();

        // Assert
        Assert.Equal(1, insertedRows);
        Assert.Null(catBefore);
        Assert.NotNull(catAfter);
        Assert.Equal(6, catAfter.Id);
    }

    [Fact]
    public void GetValue_WhenSelectOutOfOrder_GetValueReturnsRespectiveObject()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch1 = db.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
        var batch2 = db.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 2 });
        var batch3 = db.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 3 });

        var cat3 = batch3.GetValue();
        var cat2 = batch2.GetValue();
        var cat1 = batch1.GetValue();

        // Assert
        Assert.Equal(1, cat1.Id);
        Assert.Equal(2, cat2.Id);
        Assert.Equal(3, cat3.Id);
    }

    [Fact]
    public void GetValue_WhenMultipleCallsOnOneObject_ReturnsSameObjectOnRepeatedCalls()
    {
        // Arrange
        using var db = CatsDbConnectionFactory.CreateInMemoryDB();

        // Act
        var batch1 = db.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
        var batch2 = db.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 2 });

        var cat1_1 = batch1.GetValue();
        var cat1_2 = batch1.GetValue();
        var cat2 = batch2.GetValue();
        var cat1_3 = batch1.GetValue();

        // Assert
        Assert.True(ReferenceEquals(cat1_1, cat1_2));
        Assert.True(ReferenceEquals(cat1_1, cat1_3));
        Assert.False(ReferenceEquals(cat1_1, cat2));
    }
}
