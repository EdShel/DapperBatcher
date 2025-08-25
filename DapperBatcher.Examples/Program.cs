using Dapper;
using EdShel.DapperBatcher;
using Microsoft.Data.Sqlite;

using var connection = new SqliteConnection("Data Source=:memory:");
connection.Open(); // As with regular Dapper, you may omit opening the connection, but because it's an in-memory DB, we want to call Open() explicitly
InitializeDB(connection);

// Call *Batched methods - they will not execute immediately
IBatchedValue<Cat> val1 = connection.QueryFirstBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 1 });
IBatchedValue<Cat?> val2 = connection.QueryFirstOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 2 });
IBatchedValue<Cat> val3 = connection.QuerySingleBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 3 });
IBatchedValue<Cat?> val4 = connection.QuerySingleOrDefaultBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat WHERE Id = $Id", new { Id = 222 });
IBatchedValue<int> affectedRows1 = connection.ExecuteBatched("INSERT INTO Cat (Id, Name, CoatColor) VALUES ($Id, $Name, $CoatColor)", new { Id = 6, Name = "Snowball", CoatColor = "White" });
IBatchedValue<int> affectedRows2 = connection.ExecuteBatched("INSERT INTO Cat (Id, Name, CoatColor) VALUES ($Id, $Name, $CoatColor)", new { Id = 7, Name = "Snowball 2", CoatColor = "Black" });
IBatchedValue<IEnumerable<Cat>> allCats = connection.QueryBatched<Cat>("SELECT Id, Name, CoatColor FROM Cat ORDER BY Id");

// Once the first value is accessed, the entire batch gets executed
Cat cat1 = await val1.GetValueAsync(); // Prefer async for the first call but sync version works too
Cat? cat2 = val2.GetValue();
Cat cat3 = val3.GetValue();
Cat? cat4 = val4.GetValue();

Console.WriteLine($"Cat 1 QueryFirstBatched: {cat1}"); // Cat 1 QueryFirstBatched: |1: Garfield (Orange)|
Console.WriteLine($"Cat 2 QueryFirstOrDefaultBatched: {cat2}"); // Cat 2 QueryFirstOrDefaultBatched: |2: Tom (Gray)|
Console.WriteLine($"Cat 3 QuerySingleBatched: {cat3}"); // Cat 3 QuerySingleBatched: |3: Sylvester (Tuxedo)|
Console.WriteLine($"Cat 4 QuerySingleOrDefaultBatched: {cat4}"); // Cat 4 QuerySingleOrDefaultBatched: 
Console.WriteLine($"Inserted {affectedRows1.GetValue()} ({affectedRows2.GetValue()}) cats"); // Inserted 2 (2) cats
Console.WriteLine($"Now there're {allCats.GetValue().Count()} cats in total"); // Now there're 7 cats in total


static void InitializeDB(SqliteConnection connection)
{
    connection.Execute("CREATE TABLE Cat (Id INTEGER PRIMARY KEY, Name TEXT, CoatColor TEXT)");
    connection.Execute("""
        INSERT INTO Cat (Id, Name, CoatColor) VALUES
            (1, 'Garfield', 'Orange'),
            (2, 'Tom', 'Gray'),
            (3, 'Sylvester', 'Tuxedo'),
            (4, 'Felix', 'Black'),
            (5, 'Leopold', 'Orange');
    """);
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
