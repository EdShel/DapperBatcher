using System.Runtime.CompilerServices;
using Dapper;
using EdShel.DapperBatcher;
using Microsoft.Data.Sqlite;

var weakRef = new ConditionalWeakTable<object, string>();
object[] arr = new object[1000];
for (int i = 0; i < 1000; i++)
{
    arr[i] = new object();
    weakRef.Add(arr[i], "test");
}

GC.Collect();
GC.WaitForPendingFinalizers();

System.Console.WriteLine(weakRef.Count());


// using var connection = new SqliteConnection("Data Source=:memory:");
// using var connection = new SqliteConnection("Data Source=test.db");
// connection.Open(); // Open the connection before using it
// InitializeDB(connection);

// var intt = connection.QueryFirstOrDefault<int>("SELECT Id FROM Cat WHERE Id = $Id", new { Id = 1 });
// System.Console.WriteLine($"Id: {intt}");

// var val1 = connection.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
// var val2 = connection.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 2 });

// Console.WriteLine($"Cat 1: {val1.GetValue()}");
// Console.WriteLine($"Cat 2: {val2.GetValue()}");
// var cat1 = connection.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
// var cat1 = connection.QueryFirstOrDefault<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $id AND CoatColor = :coatColor", new { Id = 1, CoatColor = "Orange" });
// var cat2 = connection.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = 2");
// var cat333 = connection.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = 333");

// Console.WriteLine($"Cat 1: {cat1}");
// Console.WriteLine($"Cat 2: {cat2}");
// Console.WriteLine($"Cat 333: {cat333}");

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
