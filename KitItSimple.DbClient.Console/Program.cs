namespace KitItSimple.DbClient.Console
{
    using System;
    using System.Data.SqlClient;
    using System.Diagnostics;

    internal class Program
    {
        private const string CONNECTION_STRING
            = "Data Source=SPZVTBG;"
            + "Integrated Security=True;"
            + "Connect Timeout=30;"
            + "Encrypt=False;"
            + "TrustServerCertificate=True;"
            + "ApplicationIntent=ReadWrite;"
            + "MultiSubnetFailover=False;";

        private static void Main()
        {
            var stopwatch = Stopwatch.StartNew();
            DBClient.AddLogger(Console.WriteLine);
            var connection = DBConnection<SqlConnection>.ConfigureConnection(sqlConnection
                => sqlConnection.ConnectionString = new SqlConnectionStringBuilder
                {
                    ConnectTimeout = 30_000,
                    DataSource = "spzvtbg",
                    IntegratedSecurity = true,
                    Encrypt = false,
                    TrustServerCertificate = true,
                }
                .ConnectionString);
            var parameter = DateTime.Now;

            var result = connection
                .CreateCommand($@"SELECT @{nameof(parameter)}")
                .SetParameter(nameof(parameter), DateTime.Now)
                .ExecuteScalar<DateTime>();
            Console.WriteLine($"{result:yyyy-MM-dd HH:mm:ss.fffffff}");

            result = connection
                .BeginTransaction(transaction =>
                {
                    result = transaction
                        .CreateCommand($@"SELECT @{nameof(parameter)}")
                        .SetParameter(nameof(parameter), DateTime.Now)
                        .ExecuteScalar<DateTime>();
                    Console.WriteLine($"{result:yyyy-MM-dd HH:mm:ss.fffffff}");

                    result = transaction
                        .CreateCommand($@"SELECT @{nameof(parameter)}")
                        .SetParameter(nameof(parameter), DateTime.Now)
                        .ExecuteScalar<DateTime>();
                    Console.WriteLine($"{result:yyyy-MM-dd HH:mm:ss.fffffff}");

                    result = transaction
                        .CreateCommand($@"SELECT @{nameof(parameter)}")
                        .SetParameter(nameof(parameter), DateTime.Now)
                        .ExecuteScalar<DateTime>();
                    Console.WriteLine($"{result:yyyy-MM-dd HH:mm:ss.fffffff}");

                    result = transaction
                        .CreateCommand($@"SELECT @{nameof(parameter)}")
                        .SetParameter(nameof(parameter), null)
                        .ExecuteScalar<DateTime>();
                    Console.WriteLine($"{result:yyyy-MM-dd HH:mm:ss.fffffff}");

                    transaction.CommitTransaction();

                    return DateTime.Now;
                });

            Console.WriteLine($"{result:yyyy-MM-dd HH:mm:ss.fffffff}");
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
        }
    }
}
