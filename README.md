# GenericDbClient

**GenericDbClient** is a lightweight and fluent ADO.NET wrapper designed to simplify database access in .NET projects using any provider (SQL Server, PostgreSQL, SQLite, etc).

## ✨ Features

- Generic support for any `IDbConnection`-based provider.
- Fluent API for SQL command execution.
- Safe parameter binding.
- Lightweight reader abstraction.
- Optional logging support (coming soon).
- Async support (coming soon).

## 📦 Example Usage

```csharp
DBClient.AddLogger(Console.WriteLine);

var connection = DBConnection<SqlConnection>.ConfigureConnection(sqlConnection
    => sqlConnection.ConnectionString = new SqlConnectionStringBuilder
    {
        ConnectTimeout = 30_000,
        DataSource = ".",
        IntegratedSecurity = true,
        Encrypt = false,
        TrustServerCertificate = true,
    }
    .ConnectionString);

var results = connection
    .CreateCommand($@"
        SELECT Id
             , Name
          FROM Table
         WHERE Name LIKE @pattern")
    .SetParameter("pattern", "%doe%")
    .ExecuteReader(x => new 
    {
        Id = x.GetValue<int>(0),
        Name = x.GetValue<string>(1)
        // ...
    });

connection
    .BeginTransaction(transaction =>
    {
        var updatedRecordsCount = 0;

        foreach (var result in results)
        {
            var rowsAffected = transaction
                .CreateCommand($@"
                    UPDATE Table
                       SET Column = @{nameof(result)}
                     WHERE Id = @{nameof(result.Id)}")
                .SetParameter(nameof(result.Id), result.Id)
                .SetParameter(nameof(result), $"{result.Id}.{result.Name}")
                .ExecuteNonQuery();
            updatedRecordsCount += rowsAffected;
        }

        if (updatedRecordsCount > results.Count)
        {
            transaction.CommitTransaction();
        }
        else
        {
            transaction.RollbackTransaction();
        }
    });

## 🚀 Getting Started

```bash
dotnet add package GenericDbClient
