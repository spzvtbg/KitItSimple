namespace KitItSimple.DbClient.Console
{
    using GenericDbClient;

    using System;
    using System.Data.SqlClient;

    internal class Program
    {
        private const string CONNECTION_STRING
            = "Data Source=.;"
            + "Integrated Security=True;"
            + "Connect Timeout=30;"
            + "Encrypt=False;"
            + "TrustServerCertificate=True;"
            + "ApplicationIntent=ReadWrite;"
            + "MultiSubnetFailover=False;";

        private static void Main()
        {
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
                    var updatedCount = 0;

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
                        updatedCount += rowsAffected;
                    }

                    if (updatedCount > results.Count)
                    {
                        transaction.CommitTransaction();
                    }
                    else
                    {
                        transaction.RollbackTransaction();
                    }
                });
        }
    }
}
