using Dapper;
using EdShel.DapperBatcher;
using Microsoft.Data.Sqlite;

using var connection = new SqliteConnection("Data Source=:memory:");
connection.Open();
InitializeDB(connection);

// Call *Batched methods - they will not execute immediately
var val1 = connection.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
var val2 = connection.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 2 });

// Once the first value is accessed, the entire batch gets executed
var cat1 = val1.GetValue();
var cat2 = val2.GetValue();

Console.WriteLine($"Cat 1: {cat1}");
Console.WriteLine($"Cat 2: {cat2}");

static void InitializeDB(SqliteConnection connection)
{
    try
    {
        bool dbExists = connection.ExecuteScalar<bool>("SELECT TRUE FROM Cat LIMIT 1");
        Console.WriteLine("DB exists");
    }
    catch (SqliteException)
    {
        connection.Execute("CREATE TABLE Cat (Id INTEGER PRIMARY KEY, Name TEXT, CoatColor TEXT)");
        connection.Execute("""
            INSERT INTO Cat (Id, Name, CoatColor) VALUES
                (1, 'Garfield', 'Orange'),
                (2, 'Tom', 'Gray'),
                (3, 'Sylvester', 'Tuxedo'),
                (4, 'Felix', 'Black')
        """);
        Console.WriteLine("DB created");
    }
}

class Cat
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string CoatColor { get; set; } = null!;

    public override string ToString()
    {
        return $"|{Id}: {Name} ({CoatColor})|";
    }
}
