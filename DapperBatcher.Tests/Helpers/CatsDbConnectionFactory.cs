using System.Data;
using Dapper;
using Microsoft.Data.Sqlite;

namespace DapperBatcher.Tests.Helpers;

public static class CatsDbConnectionFactory
{
    public static IDbConnection CreateInMemoryDB()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
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
}
