using EdShel.DapperBatcher.Tests.Helpers;

namespace EdShel.DapperBatcher.Tests.BatcherExtensionsTests;

public class QueryBatchedTests
{
    [Fact]
    public void QueryBatched_ByColorAndThenById_ReturnsMultipleItems()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var batch1 = db.QueryBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE CoatColor = $CoatColor", new { CoatColor = "Orange" });
        var batch2 = db.QueryBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id < @MaxId", new { MaxId = 3 });
        var cats1 = batch1.GetValue();
        var cats2 = batch2.GetValue();

        // Assert
        Assert.Equal(2, cats1.Count());
        Assert.Equal("Orange", cats1.First().CoatColor);
        Assert.Equal("Orange", cats1.Last().CoatColor);
        Assert.Equal(2, cats2.Count());
        Assert.True(cats2.First().Id < 3);
        Assert.True(cats2.Last().Id < 3);
    }

    [Fact]
    public void QueryBatched_DifferentTypes_ReturnsCorrectClasses()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var colorsBatch = db.QueryBatched<string>("SELECT CoatColor FROM Cat WHERE CoatColor = $CoatColor", new { CoatColor = "Orange" });
        var countByColorBatch = db.QueryBatched<CoatColorGroup>("SELECT CoatColor, COUNT(*) AS CatsCount FROM Cat GROUP BY CoatColor");
        var colors = colorsBatch.GetValue();
        var countByColor = countByColorBatch.GetValue();

        // Assert
        Assert.Equal(2, colors.Count());
        Assert.All(colors, color => Assert.Equal("Orange", color));
        Assert.Contains(countByColor, x => x.CoatColor == "Orange" && x.CatsCount == 2);
        Assert.Contains(countByColor, x => x.CoatColor == "Gray" && x.CatsCount == 1);
        Assert.Contains(countByColor, x => x.CoatColor == "Tuxedo" && x.CatsCount == 1);
        Assert.Contains(countByColor, x => x.CoatColor == "Black" && x.CatsCount == 1);
    }

    private class CoatColorGroup
    {
        public string CoatColor { get; set; } = null!;
        public int CatsCount { get; set; }
    }

    [Fact]
    public void QueryBatched_DifferentTypesAndNoMatches_ReturnsEmptyIEnumerable()
    {
        // Arrange
        using var db = TestDbConnectionFactory.CreateInMemoryCatsDB();

        // Act
        var colorsBatch = db.QueryBatched<string>("SELECT CoatColor FROM Cat WHERE CoatColor = $CoatColor", new { CoatColor = "Rainbow" });
        var idsBatch = db.QueryBatched<int>("SELECT Id FROM Cat WHERE CoatColor = $CoatColor", new { CoatColor = "Rainbow" });
        var colors = colorsBatch.GetValue();
        var ids = idsBatch.GetValue();

        // Assert
        Assert.Empty(colors);
        Assert.Empty(ids);
    }
}
