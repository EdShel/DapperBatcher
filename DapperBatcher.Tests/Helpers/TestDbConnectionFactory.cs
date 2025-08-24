using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace EdShel.DapperBatcher.Tests.Helpers;

public static class TestDbConnectionFactory
{
    public static IDbConnection CreateInMemoryCatsDB()
    {
        var connection = CreateInMemoryEmptyDB();
        connection.Open();
        // Basically, 5 cats and two of them are Orange
        connection.Execute("""
            CREATE TABLE Cat (Id INTEGER PRIMARY KEY, Name TEXT, CoatColor TEXT);
            INSERT INTO Cat (Id, Name, CoatColor) VALUES
                (1, 'Garfield', 'Orange'),
                (2, 'Tom', 'Gray'),
                (3, 'Sylvester', 'Tuxedo'),
                (4, 'Felix', 'Black'),
                (5, 'Leopold', 'Orange');
        """);
        return connection;
    }

    public static IDbConnection CreateInMemoryEmptyDB()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        return connection;
    }
}
