# DapperBatcher

Lightweight & easy to use .NET library for batching database commands made through [Dapper](https://github.com/DapperLib/Dapper).

## How to use
See [examples project](./DapperBatcher.Examples/Program.cs) for the full code.

```cs
using EdShel.DapperBatcher; // Import the namespace
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
```

## API
There're a few extension methods named similarly to Dapper but they end with the prefix ``Batched``:
- ``IBatchedValue<IEnumerable<T>> QueryBatched<T>(...)``
- ``IBatchedValue<T> QueryFirstBatched<T>(...)``
- ``IBatchedValue<T?> QueryFirstOrDefaultBatched<T>(...)``
- ``IBatchedValue<T> QuerySingleBatched<T>(...)``
- ``IBatchedValue<T?> QuerySingleOrDefaultBatched<T>(...)``
- ``IBatchedValue<int> ExecuteBatched(...)``

Each method returns an interface ``IBatchedValue<T>`` which provides two functions: ``GetValueAsync`` and ``GetValue``.
When the first value is retrieved, the entire batch gets sent to the database as a single command.
After that you may obtain the values repeatedly - they are cached and returned synchronously.

### Important note about managing connections
All related ``*Batched`` calls must be made on the same connection object because the batch metadata is stored via ``ConditionalWeakTable``.
This means that the calls made for different connections are batched separately, providing better isolation and error-handling.
If you're injecting your ``IDbConnection`` via ``IServiceProvider``, then consider ``Scoped`` lifetime instead of ``Transient``.

## License
MIT but be sure to check license for [Dapper](https://github.com/DapperLib/Dapper) separately.